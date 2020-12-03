using System;
using System.Collections.Generic;
using metrics.Data.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Data.Sql
{
    public class EntityConfigurationProvider : IEntityConfigurationProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityConfigurationProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<IEntityConfiguration> GetConfigurations()
        {
            return (IEnumerable<IEntityConfiguration>)_serviceProvider.GetServices(typeof(IEntityConfiguration));
        }
    }
}