using System.Threading;
using System.Threading.Tasks;
using metrics.ML.Contracts.Data;
using metrics.ML.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.ML;

namespace metrics.ML.Services
{
    public class VkMessageMLService : BackgroundService
    {
        private readonly IMessagePredictModelService _messagePredictModelService;

        public VkMessageMLService(
            IMessagePredictModelService messagePredictModelService
        )
        {
            _messagePredictModelService = messagePredictModelService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var data = _messagePredictModelService.Load();
            var mlContext = new MLContext();
            var trainingDataView = mlContext.Data.LoadFromTextFile<VkMessageML>(@"d:\mess.csv", ',');

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

            return Task.CompletedTask;
        }
    }
}