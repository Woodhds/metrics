using System;

namespace metrics.core.Services
{
    public interface IServiceResolver
    {
        object GetService(Type type);
        TService GetService<TService>();
    }
}