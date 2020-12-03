using metrics.ML.Services.Models;
using Microsoft.ML;

namespace metrics.ML.Services.Abstractions
{
    public interface IMessagePredictModelService
    {
        MLContextModel Load();
        void Save(MLContext context, ITransformer transformer, IDataView dataView);
    }
}