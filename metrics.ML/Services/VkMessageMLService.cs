using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Abstractions;
using Base.Contracts;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.ML.Contracts.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace metrics.ML.Services
{
    public class VkMessageMLService : BackgroundService
    {
        private readonly IElasticClientFactory _elasticClientFactory;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public VkMessageMLService(
            IElasticClientFactory elasticClientFactory,
            ITransactionScopeFactory transactionScopeFactory
        )
        {
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
                    var mlContext = new MLContext();
                    var data = mlContext.Data.LoadFromEnumerable(messages);

                    ITransformer dataPrepPipeline;
                    if (File.Exists("data_preparation.zip"))
                    {
                        dataPrepPipeline = mlContext.Model.Load("data_preparation.zip", out _);
                    }
                    else
                    {
                        var dataPrepEstimator = mlContext.Transforms.Conversion
                            .MapValueToKey("Label", nameof(VkMessageML.Category))
                            .Append(mlContext.Transforms.Text.NormalizeText("NormalizedText", nameof(VkMessageML.Text)))
                            .Append(mlContext.Transforms.Text.FeaturizeText("Features", "NormalizedText"));
                        dataPrepPipeline = dataPrepEstimator.Fit(data);
                    }

                    var dataEstimator =
                        mlContext.Regression.Trainers.OnlineGradientDescent();

                    var transformedData = dataPrepPipeline.Transform(data);

                    RegressionPredictionTransformer<LinearRegressionModelParameters> trainedModel;

                    if (File.Exists("Model.zip"))
                    {
                        var originalModel = mlContext.Model.Load("Model.zip", out _);
                        var originalModelParameters =
                            ((ISingleFeaturePredictionTransformer<object>) originalModel).Model as
                            LinearRegressionModelParameters;
                        trainedModel = dataEstimator.Fit(transformedData, originalModelParameters);
                    }
                    else
                    {
                        trainedModel = dataEstimator.Fit(transformedData);
                    }

                    mlContext.Model.Save(trainedModel, transformedData.Schema, "Model.zip");

                    try
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
                    catch (Exception)
                    {
                        await scope.RollbackAsync(stoppingToken);
                    }
                }

                await Task.Delay(1000 * 60 * 60, stoppingToken);
            }
        }
    }
}