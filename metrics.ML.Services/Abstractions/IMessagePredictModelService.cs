using metrics.ML.Contracts.Data;
using Microsoft.ML;

namespace metrics.ML.Services.Abstractions
{
    public interface IMessagePredictModelService
    {
        PredictionEngine<VkMessageML, VkMessagePredict> Load();
        void Save(MLContext context, ITransformer transformer, IDataView dataView);
    }
}