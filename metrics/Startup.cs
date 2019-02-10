using Data.EF;
using DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using metrics.Options;
using metrics.Services.Abstract;
using System;
using metrics.Services.Concrete;
using metrics.Services.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json.Serialization;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.Authentication.Cookies;

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

            services.AddDbContext<DataContext>(opts => { opts.UseSqlite("Data Source=.\\lite.db;"); });
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
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(
                z => { z.SerializerSettings.ContractResolver = new DefaultContractResolver(); });

            services.AddAuthorization(z =>
            {
                z.AddPolicy("VKPolicy", e => { e.RequireClaim(Constants.VK_TOKEN_CLAIM); });
            });

            services.AddHttpClient();
            services.AddLogging();
            services.AddScoped<IBaseHttpClient, BaseHttpClient>();
            services.Configure<VkontakteOptions>(Configuration.GetSection("Vkontakte"));
            services.Configure<VKApiUrls>(Configuration.GetSection("VKApiUrls"));
            services.AddScoped<IVkClient, VkClient>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
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