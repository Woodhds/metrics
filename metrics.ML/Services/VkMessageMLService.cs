using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.ML.Contracts.Data;
using metrics.ML.Services.Abstractions;
using metrics.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.ML;

namespace metrics.ML.Services
{
    public class VkMessageMLService : BackgroundService
    {
        private readonly IVkMessageService _vkMessageService;
        private readonly IMessagePredictModelService _messagePredictModelService;

        public VkMessageMLService(IVkMessageService vkMessageService,
            IMessagePredictModelService messagePredictModelService)
        {
            _vkMessageService = vkMessageService;
            _messagePredictModelService = messagePredictModelService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var messages =
                    ((IEnumerable<VkMessage>) (await _vkMessageService.GetMessages(0, 900))
                        .Data).Where(e => e.MessageCategoryId.HasValue).Select(e => new VkMessageML
                    {
                        Category = e.MessageCategory,
                        Id = e.Id,
                        Text = e.Text,
                        OwnerId = e.Owner_Id
                    });

                var mlContext = new MLContext();
                var trainingDataView = mlContext.Data.LoadFromEnumerable(messages);
                var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(VkMessageML.Category))
                    .Append(mlContext.Transforms.Text.NormalizeText("NormalizedText", nameof(VkMessageML.Text)))
                    .Append(mlContext.Transforms.Text.FeaturizeText("Features", "NormalizedText"))
                    .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"))
                    .AppendCacheCheckpoint(mlContext);

                //mlContext.MulticlassClassification.CrossValidate(trainingDataView, pipeline);
                var trainedModel = pipeline.Fit(trainingDataView);

                _messagePredictModelService.Save(mlContext, trainedModel, trainingDataView);
                await Task.Delay(1000 * 60 * 60, stoppingToken);
            }
        }
    }
}