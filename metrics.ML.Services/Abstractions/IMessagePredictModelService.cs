using Microsoft.ML;

namespace metrics.ML.Services.Abstractions
{
    public interface IMessagePredictModelService
    {
        MLContext Load();
        void Save(MLContext context, ITransformer transformer, IDataView dataView);
    }
}