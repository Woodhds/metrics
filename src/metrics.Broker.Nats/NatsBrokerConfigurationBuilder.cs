using metrics.Broker.Abstractions;
using metrics.Broker.Nats.Pooling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker.Nats
{
    public class NatsBrokerConfigurationBuilder : IBrokerConfigurationBuilder
    {
        private readonly IServiceCollection _services;

        public NatsBrokerConfigurationBuilder(IConfiguration configuration, IServiceCollection services)
        {
            _services = services;
            _services.AddSingleton<IMessageBroker, NatsMessageBroker>();
            _services.AddSingleton<INatsConnectionFactory, NatsConnectionFactory>();
            _services.AddSingleton<INatsSubjectProvider, NatsSubjectProvider>();
            _services.AddSingleton<INatsMessageSerializer, NatsMessageSerializer>();
            var options = new NatsOptions();
            configuration.GetSection(nameof(NatsOptions)).Bind(options);
            _services.AddSingleton<INatsPool>(new NatsPool(options: opts =>
            {
                opts.Servers = options.Servers;
                opts.ReconnectWait = 2000;
            }));
        }

        public BrokerConfiguration Build()
        {
            var provider = _services.BuildServiceProvider();
            return new BrokerConfiguration(provider.GetRequiredService<IMessageBroker>(),
                new NatsHandlerConfigurator(_services), "");
        }
    }
}