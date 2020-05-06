using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using metrics.ML.Contracts.Data;
using Metrics.Ml.Services;
using metrics.ML.Services.Abstractions;

namespace metrics.ML.Services.Services
{
    public class MessagePredictService : MessagePredicting.MessagePredictingBase
    {
        private readonly IMessagePredictModelService _messagePredictModelService;

        public MessagePredictService(IMessagePredictModelService messagePredictModelService)
        {
            _messagePredictModelService = messagePredictModelService;
        }

        public override Task<MessagePredictResponse> Predict(MessagePredictRequest request, ServerCallContext context)
        {
            var predictionEngine = _messagePredictModelService.Load();

            var result = request.Messages.Select(message => new MessagePredictResponse.Types.MessagePredicted
            {
                Category = predictionEngine.Predict(new VkMessageML
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