using metrics.Broker.Abstractions;
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
            _services.Configure<NatsOptions>(configuration.GetSection(nameof(NatsOptions)));
        }

        public BrokerConfiguration Build()
        {
            var provider = _services.BuildServiceProvider();
            return new BrokerConfiguration(provider.GetRequiredService<IMessageBroker>(),
                new NatsHandlerConfigurator(_services, 
                    provider.GetRequiredService<INatsConnectionFactory>(),
                    provider.GetRequiredService<INatsSubjectProvider>(),
                    provider.GetRequiredService<INatsMessageSerializer>()), "");
        }
    }
}