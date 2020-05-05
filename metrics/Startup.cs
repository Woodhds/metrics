using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using metrics.Options;
using System.Text;
using System.Threading.Tasks;
using Base.Abstractions;
using Base.Contracts.Options;
using metrics.Broker;
using metrics.Broker.Events.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Confguraton;
using metrics.Data.Sql;
using metrics.Data.Sql.Extensions;
using metrics.Events;
using metrics.Handlers;
using metrics.logging;
using metrics.Notification.SignalR.Extensions;
using metrics.Services;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Services.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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
                .AddAuthorization(z =>
                {
                    z.AddPolicy("VKPolicy", e => { e.RequireClaim(Constants.VkTokenClaim); });
                })
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
            services.AddControllers();
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

            var jwtOptions = new JwtOptions();
            Configuration.GetSection("Jwt").Bind(jwtOptions);
            services.Configure<JwtOptions>(z =>
            {
                z.Audience = jwtOptions.Audience;
                z.Issuer = jwtOptions.Issuer;
                z.Key = jwtOptions.Key;
            });
            services.AddAuthentication(opts =>
                {
                    opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    opts.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    opts.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience
                    };
                    opts.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Query.ContainsKey("access_token"))
                            {
                                context.Token = context.Request.Query["access_token"];
                            }
                            
                            return Task.CompletedTask;
                        }
                    };
                });

            var signalROptions = new SignalROptions();
            Configuration.GetSection(nameof(SignalROptions)).Bind(signalROptions);

            services.AddMetricsSignalR(signalROptions.Host);
            
            services.AddSingleton<IBaseHttpClient, BaseHttpClient>();
            services.AddScoped<ICompetitionsService, CompetitionsService>();
            services.AddSingleton<IVkTokenAccessor, VkTokenAccessor>();
            services.Configure<VkontakteOptions>(Configuration.GetSection(nameof(VkontakteOptions)));
            services.Configure<VkApiUrls>(Configuration.GetSection("VKApiUrls"));
            services.AddSingleton<IVkClient, VkClient>();
            services.AddSingleton<IVkUserService, VkUserService>();
            services.AddSingleton<IVkMessageService, VkMessageService>();
            services.AddSingleton<IRepostCacheAccessor, RepostCacheAccessor>();
            services.AddDataContext<DataContext>(Configuration.GetConnectionString("DataContext"));
            services.AddSingleton<IEntityConfiguration, RepostEntityConfiguration>();

            services.Configure<KestrelServerOptions>(z => { z.AllowSynchronousIO = true; });

            services.AddElastic(Configuration);
            services.AddLogging(c => c.AddMetricsLogging(Configuration));

            services.AddMessageBroker(Configuration, g =>
            {
                g.Register<RepostEndEvent, RepostEndEventHandler>();
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