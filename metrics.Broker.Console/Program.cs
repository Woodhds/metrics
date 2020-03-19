using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using metrics.Broker.Events.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Confguraton;
using metrics.Data.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace metrics.Broker.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((context, serviceCollection) =>
                {
                    

                    serviceCollection.AddSingleton<IEntityConfigurationProvider, EntityConfigurationProvider>();
                    serviceCollection.AddSingleton<ITransactionScopeFactory, TransactionScopeFactory>();
                    serviceCollection.AddSingleton<IDataContextFactory, DataContextFactory>();
                    serviceCollection.AddSingleton<IEntityConfiguration, RepostEntityConfiguration>();
                    serviceCollection.AddScoped<DbContext, DataContext>();
                    serviceCollection.AddDbContextPool<DataContext>(x =>
                    {
                        x.EnableSensitiveDataLogging();
                        x.UseNpgsql(context.Configuration.GetConnectionString("DataContext"));
                        x.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                    });
                    
                    serviceCollection.AddMessageBroker(context.Configuration,
                        provider =>
                        {
                            provider.Register<RepostEvent, RepostEventHandler>();
                            provider.Register<LoginEvent, LoginEventHandler>();
                            provider.Register<GroupJoinEvent, GroupJoinEventHandler>();
                        });
                });

        static async Task GroupJoin(int id, int userId)
        {
        }

        static async Task RepostMessage(int ownerId, int id, int userId)
        {
        }
    }
}