using metrics.ML.Contracts.Data;
using metrics.ML.Services.Abstractions;
using Microsoft.ML;

namespace metrics.ML.Services.Services
{
    public class MessagePredictModelService : IMessagePredictModelService
    {
        public MLContext Load()
        {
            var context = new MLContext();
            context.Model.Load("Model.zip", out var _);
            return context;
        }

        public void Save(MLContext context, ITransformer transformer, IDataView dataView)
        {
            context.Model.Save(transformer, dataView.Schema, "Model.zip");
        }
    }
}