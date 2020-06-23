using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Abstractions;
using Base.Contracts;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.ML.Contracts.Data;
using metrics.ML.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.ML;

namespace metrics.ML.Services
{
    public class VkMessageMLService : BackgroundService
    {
        private readonly IMessagePredictModelService _messagePredictModelService;
        private readonly IElasticClientFactory _elasticClientFactory;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public VkMessageMLService(
            IMessagePredictModelService messagePredictModelService,
            IElasticClientFactory elasticClientFactory,
            ITransactionScopeFactory transactionScopeFactory
        )
        {
            _messagePredictModelService = messagePredictModelService;
            _elasticClientFactory = elasticClientFactory;
            _transactionScopeFactory = transactionScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = await _transactionScopeFactory.CreateAsync(cancellationToken: stoppingToken);
                var messageVks = scope.GetRepository<MessageVk>().Read()
                    .Select(e => new
                    {
                        e.OwnerId,
                        e.MessageId,
                        MessageCategory = e.MessageCategory.Title,
                        e.MessageCategoryId
                    })
                    .ToList();

                var messages = (await _elasticClientFactory.Create().SearchAsync<VkMessage>(descriptor =>
                        descriptor.Query(containerDescriptor => containerDescriptor.Ids(idsQueryDescriptor =>
                                idsQueryDescriptor.Values(messageVks.Select(f =>
                                    (f.OwnerId ^ f.MessageId).ToString())))).Skip(0)
                            .Take(9000),
                    stoppingToken)).Documents.Join(messageVks, message => message.Identifier,
                    vk => (vk.OwnerId ^ vk.MessageId),
                    (e, vk) =>
                        new VkMessageML
                        {
                            Category = vk.MessageCategory,
                            Id = e.Id,
                            Text = e.Text,
                            OwnerId = e.Owner_Id
                        }).ToArray();

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
                    
                    /*try
                    {
                        foreach (var message in messageVks)
                        {
                            await scope.GetRepository<MessageVk>().UpdateAsync(new MessageVk
                            {
                                Status = MessageVkStatus.Processed,
                                MessageCategoryId = message.MessageCategoryId,
                                MessageId = message.MessageId,
                                OwnerId = message.OwnerId
                            });
                        }

                        await scope.CommitAsync(stoppingToken);
                    }
                    catch (Exception e)
                    {
                        await scope.RollbackAsync(stoppingToken);
                    }
                    */
                }

                await Task.Delay(1000 * 60 * 60, stoppingToken);
            }
        }
    }
}