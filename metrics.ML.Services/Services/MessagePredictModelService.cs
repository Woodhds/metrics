using metrics.ML.Contracts.Data;
using metrics.ML.Services.Abstractions;
using Microsoft.ML;

namespace metrics.ML.Services.Services
{
    public class MessagePredictModelService : IMessagePredictModelService
    {
        public PredictionEngine<VkMessageML, VkMessagePredict> Load()
        {
            var context = new MLContext();
            var trainedModel = context.Model.Load("Model.zip", out var _);
            return context.Model.CreatePredictionEngine<VkMessageML, VkMessagePredict>(trainedModel);
        }

        public void Save(MLContext context, ITransformer transformer, IDataView dataView)
        {
            context.Model.Save(transformer, dataView.Schema, "Model.zip");
        }
    }
}