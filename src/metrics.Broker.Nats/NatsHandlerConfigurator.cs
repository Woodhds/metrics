using System;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker.Nats
{
    public class NatsHandlerConfigurator : IHandlerConfigurator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INatsConnectionFactory _natsConnectionFactory;
        private readonly INatsSubjectProvider _natsSubjectProvider;
        private readonly INatsMessageSerializer _natsMessageSerializer;

        public NatsHandlerConfigurator(IServiceCollection serviceCollection,
            INatsConnectionFactory natsConnectionFactory, INatsSubjectProvider natsSubjectProvider, INatsMessageSerializer natsMessageSerializer)
        {
            _natsConnectionFactory = natsConnectionFactory;
            _natsSubjectProvider = natsSubjectProvider;
            _natsMessageSerializer = natsMessageSerializer;
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public void ConfigureConsumer<TEvent>() where TEvent : class, new()
        {
            _natsConnectionFactory.CreateConnection().SubscribeAsync(_natsSubjectProvider.GetSubject<TEvent>(),
                (sender, args) =>
                    new NatsMessageHandler<TEvent>(_serviceProvider.GetService<IMessageHandler<TEvent>>(),
                        _natsMessageSerializer).HandleAsync(args.Message)).Start();
        }

        public void ConfigureCommandConsumer<TEvent>() where TEvent : class, new()
        {
            ConfigureConsumer<TEvent>();
        }

        public void ConfigureCommand<TCommand>(string host) where TCommand : class, new()
        {
        }
    }
}