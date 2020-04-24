using Base.Contracts.Options;
using Microsoft.Extensions.DependencyInjection;
using metrics.Broker.Events.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Confguraton;
using metrics.Data.Sql;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Services.Utils;
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
                    serviceCollection.AddHostedService<RepostHostedService>();
                    serviceCollection.AddSingleton<IVkClient, VkClient>();
                    serviceCollection.Configure<VkApiUrls>(context.Configuration.GetSection(nameof(VkApiUrls)));
                    serviceCollection.AddHttpClient();
                    serviceCollection.AddSingleton<IBaseHttpClient, BaseHttpClient>();
                    serviceCollection.AddSingleton<IVkTokenAccessor, CacheTokenAccessor>();
                    serviceCollection.AddSingleton<IRepostCacheAccessor, RepostCacheAccessor>();
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
                            provider.Register<RepostGroupCreatedEvent, RepostEventGroupCreatedHandler>();
                            provider.Register<LoginEvent, LoginEventHandler>();
                            provider.Register<RepostedEvent, RepostedEventHandler>();
                        });
                });
    }
}