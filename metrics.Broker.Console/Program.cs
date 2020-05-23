using Base.Contracts.Options;
using metrics.Authentication.Options;
using Microsoft.Extensions.DependencyInjection;
using metrics.Broker.Events.Events;
using metrics.Cache;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Confguraton;
using metrics.Data.Sql;
using metrics.Data.Sql.Extensions;
using metrics.Identity.Client;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Services.Utils;
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
                    serviceCollection.AddSingleton<IEntityConfiguration, RepostEntityConfiguration>();
                    serviceCollection.AddDataContext<DataContext>(
                        context.Configuration.GetConnectionString("DataContext"));
                    serviceCollection.AddCaching(context.Configuration);
                    serviceCollection.AddIdentityClient(context.Configuration);
                    serviceCollection.Configure<JwtOptions>(context.Configuration.GetSection(nameof(JwtOptions)));
                    serviceCollection.Configure<VkontakteOptions>(
                        context.Configuration.GetSection(nameof(VkontakteOptions)));
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