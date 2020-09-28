using System.Security.Claims;
using System.Threading.Tasks;
using Base.Contracts.Options;
using metrics.Authentication;
using metrics.Broker;
using metrics.Broker.Rabbitmq;
using metrics.Identity.Data;
using metrics.Identity.Extensions;
using metrics.Identity.Infrastructure.Identity;
using metrics.Identity.Options;
using metrics.Identity.Services;
using metrics.Serialization;
using metrics.Web.Conventions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace metrics.Identity
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services
                .AddMvcCore()
                .AddCors(opts =>
                {
                    opts.AddPolicy("CorsPolicy", builder =>
                        builder
                            .WithOrigins(_configuration["FrontendUrl"])
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .AllowAnyMethod()
                    );
                })
                .AddJsonOptions(options =>
                {
                    new JsonSerializerOptionsProvider().Apply(options.JsonSerializerOptions);
                });

            services.AddHttpContextAccessor();

            services
                .AddControllers(x => { x.UseGeneralRoutePrefix("api"); });

            var vkOptions = new VkontakteOptions();
            _configuration.GetSection(nameof(VkontakteOptions)).Bind(vkOptions);
            services.Configure<VkontakteOptions>(_configuration.GetSection(nameof(VkontakteOptions)));

            services.AddMetricsAuthentication(_configuration, builder =>
            {
                builder.AddCookie()
                    .AddOAuth<VkOauthOptions, VkHandler>("Vkontakte", "Vkontakte", options =>
                    {
                        options.SignInScheme = IdentityConstants.ExternalScheme;
                        options.ClientId = vkOptions.AppId;
                        options.ClientSecret = vkOptions.AppSecret;
                        options.CallbackPath = new PathString("/sign-in");
                        options.Scope.Add(vkOptions.AppScope);
                        options.Fields = vkOptions.Fields;
                        options.ApiVersion = vkOptions.ApiVersion;
                        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "first_name");
                        options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "last_name");
                        options.ClaimActions.MapJsonKey("photo", "photo_50");
                        options.SaveTokens = true;
                    });
            });
            
            services.AddHttpClient();

            services.AddMetricsIdentity(_configuration.GetConnectionString(nameof(IdentityContext)));
            
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Identity API", Version = "v1"});
            });

            services.AddMessageBroker(_configuration, (collection, configuration) => new RabbitMqBrokerConfigurationBuilder(_configuration, services));
            services.AddGrpc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseForwardedHeaders();
            }

            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API V1"); });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<IdentityTokenService>();
                endpoints.MapControllers();
            });
        }
    }
}