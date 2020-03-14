using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using Microsoft.Extensions.Configuration;

namespace metrics.Broker.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../appsettings.json"));

            serviceCollection.AddMessageBroker(configBuilder.Build(),
                provider => { provider.Register<RepostEvent, RepostEventHandler>(); });

            var serviceProvider = serviceCollection.BuildServiceProvider(true);
            var messageBroker = serviceProvider.GetService<IMessageBroker>();

            foreach (var i in Enumerable.Range(1, 100))
            {
                await messageBroker.PublishAsync(new RepostEvent {Id = i});
            }
        }
    }
}