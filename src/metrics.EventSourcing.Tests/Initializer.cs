using System;
using metrics.EventSourcing.Abstractions.Query;
using metrics.EventSourcing.Tests.Events;
using metrics.EventSourcing.Tests.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.EventSourcing.Tests
{
    public static class Initializer
    {
        public static readonly IServiceProvider ServiceProvider;
        
        static Initializer()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IQueryHandler<TestQuery, string>, TestQueryHandler>();
            serviceCollection.AddSingleton<IQueryProcessor, QueryProcessor>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}