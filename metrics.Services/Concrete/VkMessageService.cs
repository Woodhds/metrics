using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Contracts;
using Base.Contracts.Models;
using Elastic.Client;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using Metrics.Ml.Services;
using metrics.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace metrics.Services.Concrete
{
    public class VkMessageService : IVkMessageService
    {
        private readonly IElasticClientFactory _elasticClientFactory;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly MessagePredicting.MessagePredictingClient _messagePredictingClient;
        private readonly ILogger<VkMessageService> _logger;

        public VkMessageService(
            IElasticClientFactory elasticClientFactory,
            ITransactionScopeFactory transactionScopeFactory,
            MessagePredicting.MessagePredictingClient messagePredictingClient,
            ILogger<VkMessageService> logger
        )
        {
            _elasticClientFactory = elasticClientFactory;
            _transactionScopeFactory = transactionScopeFactory;
            _messagePredictingClient = messagePredictingClient;
            _logger = logger;
        }

        public async Task<DataSourceResponseModel> GetMessages(int page = 0, int take = 50, string? search = null)
        {
            var response = await _elasticClientFactory
                .Create()
                .SearchAsync<VkMessageModel>(z => z
                    .From(page * take)
                    .Take(take)
                    .Query(f =>
                    {
                        var q = f
                            .Bool(e => e
                                .Filter(g => g
                                    .MatchPhrase(n => n
                                        .Field(message => message.Text)
                                        .Query(search)
                                    )
                                )
                            );

                        return q;
                    })
                );

            using var scope = _transactionScopeFactory.CreateQuery();
            var keys = response.Documents.Select(f => f.OwnerId + "_" + f.Id);
            var items = await scope.Query<MessageVk>()
                .Select(r =>
                    new {Key = r.OwnerId.ToString() + "_" + r.MessageId.ToString(), item = r})
                .Where(e => keys.Contains(e.Key))
                .Select(f => new {message = f.item, category = f.item.MessageCategory.Title})
                .ToListAsync();

            var reposts = await scope.Query<VkRepost>()
                .Where(f => f.Status == VkRepostStatus.Complete)
                .Select(r =>
                    new {Key = r.OwnerId.ToString() + "_" + r.MessageId.ToString(), r.OwnerId, r.MessageId})
                .Where(f => keys.Contains(f.Key))
                .ToListAsync();

            MessagePredictResponse? predicted = null;
            try
            {
                predicted = await _messagePredictingClient.PredictAsync(new MessagePredictRequest
                {
                    Messages =
                    {
                        response.Documents.Select(e => new MessagePredictRequest.Types.MessagePredict
                        {
                            Id = e.Id,
                            Text = e.Text,
                            OwnerId = e.OwnerId
                        })
                    }
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to access predict client");
            }

            foreach (var document in response.Documents)
            {
                var messageCategory =
                    items.FirstOrDefault(f =>
                        f.message.MessageId == document.Id && f.message.OwnerId == document.OwnerId);
                document.MessageCategoryId = messageCategory?.message?.MessageCategoryId;
                document.MessageCategory = messageCategory?.category;
                document.UserReposted = reposts.Any(f => f.MessageId == document.Id && f.OwnerId == document.OwnerId);
                document.MessageCategoryPredict = predicted?.Messages
                    .FirstOrDefault(e => e.Id == document.Id && e.OwnerId == document.OwnerId)?.Category;
            }

            //await WriteLog(response.Documents);
            return new DataSourceResponseModel(response.Documents, response.Total);
        }

        private async Task WriteLog(IEnumerable<VkMessageModel> messages)
        {
            var sb = new StringBuilder();
            messages.Select((v, i) => new { v,i }).ToList().ForEach(a => sb.AppendLine($"{a.i}.  https://vk.com/wall{a.v.OwnerId}_{a.v.Id}"));

            await File.WriteAllTextAsync(@"D:\log.txt", sb.ToString());
        }
    }
}