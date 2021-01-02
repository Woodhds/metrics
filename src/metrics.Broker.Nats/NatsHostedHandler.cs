using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.Hosting;
using NATS.Client;

namespace metrics.Broker.Nats
{
    public class NatsHostedHandler<TEvent> : IHostedService where TEvent : class, new()
    {
        private readonly IAsyncSubscription _subscription;

        public NatsHostedHandler(
            INatsConnectionFactory natsConnectionFactory,
            INatsSubjectProvider natsSubjectProvider,
            INatsMessageSerializer natsMessageSerializer,
            IMessageHandler<TEvent> messageHandler)
        {
            using var connection = natsConnectionFactory.CreateConnection();
            _subscription = connection.SubscribeAsync(
                natsSubjectProvider.GetSubject<TEvent>(),
                (_, args) =>
                    new NatsMessageHandler<TEvent>(messageHandler, natsMessageSerializer).HandleAsync(
                        args.Message));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscription.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription.Unsubscribe();

            return Task.CompletedTask;
        }
    }
}