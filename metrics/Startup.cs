using Data.EF;
using DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using metrics.Options;
using metrics.Services.Abstract;
using System;
using metrics.Services.Concrete;
using metrics.Services.Options;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace metrics
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<DataContext>(opts =>
            {
                opts.UseSqlite(Configuration.GetConnectionString("DataContext"));
            });
            services.AddScoped<DbContext, DataContext>();
            services.AddHttpContextAccessor();

            services.Configure<JwtOptions>(Configuration.GetSection("Jwt"));
            services.AddAuthentication(opts =>
                {
                    opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opts.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opts.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opts.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(opts =>
                {
                    opts.LoginPath = new PathString("/Account/Login");
                    opts.LogoutPath = new PathString("/Account/Logout");
                });

            services.ConfigureApplicationCookie(z =>
            {
                z.LoginPath = "/Account/Login";
                z.ReturnUrlParameter = "returnUrl";
                z.Cookie.HttpOnly = true;
                z.SlidingExpiration = true;
                z.ExpireTimeSpan = TimeSpan.FromDays(1);
            });


            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddMvc(a =>
            {
                a.EnableEndpointRouting = false;
            }).AddNewtonsoftJson(z =>
            {
                z.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddAuthorization(z =>
            {
                z.AddPolicy("VKPolicy", e => { e.RequireClaim(Constants.VK_TOKEN_CLAIM); });
            });

            services.AddHttpClient();
            services.AddScoped<IBaseHttpClient, BaseHttpClient>();
            services.AddScoped<ICompetitionsService, CompetitionsService>();
            services.Configure<VkontakteOptions>(Configuration.GetSection("Vkontakte"));
            services.Configure<VKApiUrls>(Configuration.GetSection("VKApiUrls"));
            services.AddScoped<IVkClient, VkClient>();
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<KestrelServerOptions>(z => { z.AllowSynchronousIO = true; });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvcWithDefaultRoute();

            DataBaseInitializer.Init(serviceProvider);
        }
    }
}