using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MassTransit;
using metrics.Broker.Abstractions;

namespace metrics.Broker
{
    public class MessageHandler<T> : IConsumer<T> where T : class
    {
        private readonly IMessageHandler<T> _handler;

        public MessageHandler([NotNull]IMessageHandler<T> handler)
        {
            _handler = handler;
        }

        public Task Consume(ConsumeContext<T> context)
        {
            return _handler.HandleAsync(context.Message, context.CancellationToken);
        }
    }
}