using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using metrics.Options;
using Base.Abstractions;
using Base.Contracts.Options;
using metrics.Authentication;
using metrics.Broker;
using metrics.Broker.Events.Events;
using metrics.Cache;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Confguraton;
using metrics.Data.Sql;
using metrics.Data.Sql.Extensions;
using metrics.Events;
using metrics.Handlers;
using metrics.Identity.Client;
using metrics.logging;
using metrics.ML.Services.Extensions;
using metrics.Notification.SignalR.Extensions;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Services.Utils;
using metrics.Web.Conventions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace metrics
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private readonly string CorsPolicy = nameof(CorsPolicy);

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddCors(options =>
                {
                    options.AddPolicy(CorsPolicy, z =>
                    {
                        z.WithOrigins("http://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
                })
                .AddAuthorization()
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
            services.AddControllers(q =>
            {
                q.UseGeneralRoutePrefix("api");
            });
            
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMetricsAuthentication(Configuration);

            var signalROptions = new SignalROptions();
            Configuration.GetSection(nameof(SignalROptions)).Bind(signalROptions);

            services.AddMetricsSignalR(signalROptions.Host);
            
            services.AddSingleton<IBaseHttpClient, BaseHttpClient>();
            services.AddScoped<ICompetitionsService, CompetitionsService>();
            services.AddScoped<IVkTokenAccessor, CacheTokenAccessor>();
            services.Configure<VkontakteOptions>(Configuration.GetSection(nameof(VkontakteOptions)));
            services.Configure<VkApiUrls>(Configuration.GetSection("VKApiUrls"));
            services.AddScoped<IVkClient, VkClient>();
            services.AddScoped<IVkUserService, VkUserService>();
            services.AddSingleton<IVkMessageService, VkMessageService>();
            services.AddSingleton<IRepostCacheAccessor, RepostCacheAccessor>();
            services.AddDataContext<DataContext>(Configuration.GetConnectionString("DataContext"));
            services.AddSingleton<IEntityConfiguration, RepostEntityConfiguration>();
            services.AddPredictClient("https://localhost:5006");
            services.AddCaching(Configuration);

            services.Configure<KestrelServerOptions>(z => { z.AllowSynchronousIO = true; });

            services.AddElastic(Configuration);
            services.AddLogging(c => c.AddMetricsLogging(Configuration));
            services.AddIdentityClient(Configuration);

            services.AddMessageBroker(Configuration, g =>
            {
                g.Register<NotifyUserEvent, NotifyUserEventHandler>();
                g.Register<SetMessageTypeEvent, SetTypeEventHandler>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseCors(CorsPolicy);
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.AddMetricsSignalR(CorsPolicy);
            });
        }
    }
}