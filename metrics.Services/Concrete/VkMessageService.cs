using System;
using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using Elastic.Client;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using Metrics.Ml.Services;
using metrics.Services.Abstractions;
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
            MessagePredicting.MessagePredictingClient messagePredictingClient, ILogger<VkMessageService> logger)
        {
            _elasticClientFactory = elasticClientFactory;
            _transactionScopeFactory = transactionScopeFactory;
            _messagePredictingClient = messagePredictingClient;
            _logger = logger;
        }

        public async Task<DataSourceResponseModel> GetMessages(int page = 0, int take = 50, string search = null,
            string user = null)
        {
            var response = await _elasticClientFactory
                .Create()
                .SearchAsync<VkMessage>(z => z
                    .From(page * take)
                    .Take(take)
                    .Query(f =>
                    {
                        var q = f
                            .Bool(e => e
                                .Filter(g => g
                                    .MatchPhrase(q => q
                                        .Field(message => message.Text)
                                        .Query(search)
                                    )
                                )
                            );
                        if (string.IsNullOrEmpty(user)) return q;

                        return q &&
                               f
                                   .Term(t => t
                                       .Value(user)
                                       .Field(l => l.RepostedFrom)
                                   );
                    })
                );
            using var scope = _transactionScopeFactory.CreateQuery();
            var keys = response.Documents.Select(f => f.Owner_Id + "_" + f.Id);
            var items = scope.Query<MessageVk>().Select(r =>
                    new {Key = r.OwnerId.ToString() + "_" + r.MessageId.ToString(), item = r})
                .Where(e => keys.Contains(e.Key))
                .Select(f => new {message = f.item, category = f.item.MessageCategory.Title}).ToList();

            MessagePredictResponse predicted = null;
            try
            {
                predicted = await _messagePredictingClient.PredictAsync(new MessagePredictRequest
                {
                    Messages =
                    {
                        response.Documents.Select(e => new MessagePredictRequest.Types.MessagePredict
                            {Id = e.Id, Text = e.Text, OwnerId = e.Owner_Id})
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
                        f.message.MessageId == document.Id && f.message.OwnerId == document.Owner_Id);
                document.MessageCategoryId = messageCategory?.message?.MessageCategoryId;
                document.MessageCategory = messageCategory?.category;
                document.MessageCategoryPredict = predicted?.Messages
                    .FirstOrDefault(e => e.Id == document.Id && e.OwnerId == document.Owner_Id)?.Category;
            }

            return new DataSourceResponseModel(response.Documents, response.Total);
        }
    }
}