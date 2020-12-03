using metrics.ML.Services.Abstractions;
using metrics.ML.Services.Models;
using Microsoft.ML;

namespace metrics.ML.Services.Services
{
    public class MessagePredictModelService : IMessagePredictModelService
    {
        public MLContextModel Load()
        {
            var context = new MLContext();
            var iTransformer = context.Model.Load("Model.zip", out var schema);
            return new MLContextModel
            {
                Context = context,
                Transformer = iTransformer,
                Schema = schema
            };
        }

        public void Save(MLContext context, ITransformer transformer, IDataView dataView)
        {
            context.Model.Save(transformer, dataView.Schema, "Model.zip");
        }
    }
}