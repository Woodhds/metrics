using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using metrics.ML.Contracts.Data;
using Metrics.Ml.Services;
using Microsoft.Extensions.ML;

namespace metrics.ML.Services.Services
{
    public class MessagePredictService : MessagePredicting.MessagePredictingBase
    {
        private readonly PredictionEnginePool<VkMessageML, VkMessagePredict>
            _predictionEnginePool;

        public MessagePredictService(
            PredictionEnginePool<VkMessageML, VkMessagePredict> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        public override Task<MessagePredictResponse> Predict(MessagePredictRequest request, ServerCallContext context)
        {
            var result = request.Messages.Select(message => new MessagePredictResponse.Types.MessagePredicted
            {
                Category = _predictionEnginePool.Predict(new VkMessageML
                {
                    Id = message.Id,
                    Text = message.Text,
                    OwnerId = message.OwnerId
                })?.Category,
                Id = message.Id,
                OwnerId = message.OwnerId
            }).ToList();

            return Task.FromResult(new MessagePredictResponse
            {
                Messages = {result}
            });
        }
    }
}