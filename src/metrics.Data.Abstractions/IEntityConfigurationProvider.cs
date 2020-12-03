using System.Collections.Generic;

namespace metrics.Data.Abstractions
{
    public interface IEntityConfigurationProvider
    {
        IEnumerable<IEntityConfiguration> GetConfigurations();
    }
}