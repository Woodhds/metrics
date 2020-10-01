using System;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker.Abstractions
{
    public interface IBrokerConfigurationBuilder
    {
        BrokerConfiguration Build(IServiceCollection serviceProvider);
    }
}