using DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using metrics.Options;
using metrics.Services.Abstract;
using System;
using System.Text;
using System.Threading.Tasks;
using Base.Abstractions;
using Base.Contracts.Options;
using metrics.Services.Concrete;
using metrics.Services.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddHttpContextAccessor();
            services.AddControllers().AddNewtonsoftJson(opts =>
            {
                opts.SerializerSettings.ContractResolver = new DefaultContractResolver();
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
                });


            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddAuthorization(z =>
            {
                z.AddPolicy("VKPolicy", e => { e.RequireClaim(Constants.VK_TOKEN_CLAIM); });
            });
            services.AddSignalR();

            services.AddHttpClient();
            services.AddSingleton<IBaseHttpClient, BaseHttpClient>();
            services.AddScoped<ICompetitionsService, CompetitionsService>();
            services.AddSingleton<IVkTokenAccessor, VkTokenAccessor>();
            services.Configure<VkontakteOptions>(Configuration.GetSection("Vkontakte"));
            services.Configure<VKApiUrls>(Configuration.GetSection("VKApiUrls"));
            services.Configure<ElasticOptions>(Configuration.GetSection("ElasticOptions"));
            services.AddSingleton<IVkClient, VkClient>();
            services.AddSingleton<IElasticClientProvider, ElasticClientProvider>();
            services.AddSingleton<IVkUserService, VkUserService>();

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<KestrelServerOptions>(z => { z.AllowSynchronousIO = true; });

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy, z =>
                {
                    z.WithOrigins("http://localhost:4200", "http://localhost:5000", "https://localhost:5001").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
                app.UseSpaStaticFiles();
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHub<NotificationHub>("/notifications").RequireAuthorization("VkPolicy");
            });

            app.UseCors(CorsPolicy);
            
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        }
    }
}