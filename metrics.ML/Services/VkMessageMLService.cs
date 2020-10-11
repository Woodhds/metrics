using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using Base.Contracts.Models;
using Elastic.Client;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.ML.Contracts.Data;
using metrics.ML.Services.Abstractions;
using metrics.Services.Abstractions;
using metrics.Services.Abstractions.VK;
using Microsoft.Extensions.Hosting;
using Microsoft.ML;
using Nest;

namespace metrics.ML.Services
{
    public class VkMessageMLService : BackgroundService
    {
        private readonly IMessagePredictModelService _messagePredictModelService;
        private readonly IElasticClientFactory _elasticClientFactory;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IVkWallService _vkClient;

        public VkMessageMLService(
            IMessagePredictModelService messagePredictModelService,
            IElasticClientFactory elasticClientFactory,
            ITransactionScopeFactory transactionScopeFactory,
            IVkWallService vkClient
        )
        {
            _messagePredictModelService = messagePredictModelService;
            _elasticClientFactory = elasticClientFactory;
            _transactionScopeFactory = transactionScopeFactory;
            _vkClient = vkClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _transactionScopeFactory.CreateQuery(stoppingToken);
                var messageVks = scope.Query<MessageVk>()
                    .Select(e => new
                    {
                        e.OwnerId,
                        e.MessageId,
                        MessageCategory = e.MessageCategory.Title,
                        e.MessageCategoryId
                    })
                    .ToList();

                var messages = messageVks.GroupJoin((await _elasticClientFactory.Create().SearchAsync<VkMessageModel>(
                        descriptor =>
                            descriptor.Query(containerDescriptor => containerDescriptor.Ids(idsQueryDescriptor =>
                                    idsQueryDescriptor.Values(messageVks.Select(f =>
                                        (f.OwnerId ^ f.MessageId).GetHashCode().ToString())))).Skip(0)
                                .Take(9000),
                        stoppingToken)).Documents,
                    vk => (vk.OwnerId ^ vk.MessageId).GetHashCode(),
                    message => message.Identifier, (vk, e) => new VkMessageML
                    {
                        Category = vk.MessageCategory,
                        Id = e.FirstOrDefault()?.Id ?? vk.MessageId,
                        Text = e.FirstOrDefault()?.Text,
                        OwnerId = e.FirstOrDefault()?.OwnerId ?? vk.OwnerId
                    }).ToList();

                var toFetch = messages.Where(f => string.IsNullOrEmpty(f.Text)).ToArray();
                var i = 0;
                while (i < toFetch.Length)
                {
                    var data = await _vkClient.GetById(toFetch.Skip(i).Take(100)
                        .Select(f => new VkRepostViewModel(f.OwnerId, f.Id)));

                    if (data.Response?.Items.Count > 0)
                    {
                        await _elasticClientFactory.Create()
                            .IndexManyAsync(
                                data.Response?.Items.Select(f => new VkMessageModel(f, data.Response.Groups)),
                                cancellationToken: stoppingToken
                            );
                    }

                    foreach (var entry in data.Response.Items)
                    {
                        var idx = messages.FindIndex(a => a.Id == entry.Id && a.OwnerId == entry.OwnerId);
                        if (idx > -1)
                        {
                            messages[idx].Text = entry.Text;
                        }
                    }
                    
                    i += 100;
                }

                messages = messages.Where(f => !string.IsNullOrEmpty(f.Text)).ToList();
                if (messages.Any())
                {
                    var data = _messagePredictModelService.Load();
                    var mlContext = new MLContext();
                    var trainingDataView = mlContext.Data.LoadFromEnumerable(messages);

                    var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(VkMessageML.Category))
                        .Append(mlContext.Transforms.Text.NormalizeText("NormalizedText", nameof(VkMessageML.Text)))
                        .Append(mlContext.Transforms.Text.FeaturizeText("Features", "NormalizedText"))
                        .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                        .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"))
                        .AppendCacheCheckpoint(mlContext);

                    var transformedNewData = data.Transformer.Transform(trainingDataView);
                    mlContext.MulticlassClassification.CrossValidate(transformedNewData, pipeline);
                    var trainedModel = pipeline.Fit(transformedNewData);

                    _messagePredictModelService.Save(mlContext, trainedModel, transformedNewData);

                }

                await Task.Delay(1000 * 60 * 60, stoppingToken);
            }
        }
    }
}