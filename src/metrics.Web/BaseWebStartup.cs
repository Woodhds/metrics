using metrics.Authentication;
using metrics.Broker.Abstractions;
using metrics.Web.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace metrics.Web
{
    public abstract class BaseWebStartup : BaseStartup
    {
        protected readonly string CorsPolicy = nameof(CorsPolicy);
        protected BaseWebStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureDataContext(IServiceCollection services)
        {
        }

        protected override void ConfigureMvc(IServiceCollection services)
        {
            base.ConfigureMvc(services);
            services.AddMvcCore()
                .AddCors(options =>
                {
                    options.AddPolicy(CorsPolicy, z =>
                    {
                        z.WithOrigins(Configuration["FrontendUrl"])
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
                })
                .AddJsonOptions(x =>
                {
                    JsonSerializerOptionsProvider.Apply(x.JsonSerializerOptions);
                });
            services.AddControllers(q => { q.UseGeneralRoutePrefix("api"); });
        }

        protected override void ConfigureAuthentication(IServiceCollection services)
        {
            base.ConfigureAuthentication(services);
            services.AddMetricsAuthentication(Configuration);
        }

        protected override void ConfigureBase(IServiceCollection services)
        {
            base.ConfigureBase(services);
            
            services.AddHttpContextAccessor();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

            services.Configure<KestrelServerOptions>(z => { z.AllowSynchronousIO = true; });
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifeTime)
        {
            base.Configure(app, env, lifeTime);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseCors(CorsPolicy);
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseAuthorization();
            
            ConfigureManualMiddleware(app);
            
            app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.json", "Metrics API V1"); });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                ConfigureEndpoints(endpoints);
            });
        }

        protected override void AddBrokerHandlers(IMessageHandlerProvider provider)
        {
        }

        protected override void ConfigureApplicationServices(IServiceCollection services)
        {
        }

        protected virtual void ConfigureManualMiddleware(IApplicationBuilder builder)
        {
            
        }
        
        protected abstract void ConfigureEndpoints(IEndpointRouteBuilder endpoints);
    }
}