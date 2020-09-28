using System;

namespace metrics.Broker.Abstractions
{
    public interface IBrokerConfigurationBuilder
    {
        BrokerConfiguration Build(IServiceProvider serviceProvider);
    }
}