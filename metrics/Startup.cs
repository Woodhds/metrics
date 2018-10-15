using Data.EF;
using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using metrics.Options;
using metrics.Services;
using metrics.Services.Abstract;
using System;
using DAL.Identity;
using metrics.Services.Concrete;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SpaServices.AngularCli;

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

            var connectionString = Configuration.GetConnectionString("DataContext");
            services.AddDbContext<DataContext>(opts => {
                opts.UseNpgsql(connectionString);
            });
            services.AddScoped<DbContext, DataContext>();
            services.AddHttpContextAccessor();

            services.AddDefaultIdentity<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
            })
            .AddRoles<Role>()
            .AddRoleManager<RoleManager<Role>>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<DataContext>();


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)

                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                {
                    opts.Audience = "metrics";
                    opts.ClaimsIssuer = "metrics";
                })
                .AddCookie();
                //.AddOAuth("vk", e =>
                //{
                //    e.ClientId = Configuration.GetValue<string>("Vkontakte:ClientId");
                //    e.ClientSecret = Configuration.GetValue<string>("Vkontakte:ClientSecret");
                //    e.AuthorizationEndpoint = "https://oauth.vk.com/authorize";
                //    e.TokenEndpoint = "https://oauth.vk.com/access_token";
                //    e.CallbackPath = "/account/signin-vkontakte";
                //    e.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "user_id");
                //});

            services.ConfigureApplicationCookie(z =>
            {
                z.LoginPath = "/Account/Login";
                z.ReturnUrlParameter = "returnUrl";
                z.Cookie.HttpOnly = true;
                z.SlidingExpiration = true;
                z.ExpireTimeSpan = TimeSpan.FromDays(1);
            });

            
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).ConfigureApplicationPartManager(
                manager => { manager.FeatureProviders.Add(new GenericControllerFeatureProvider()); });
            services.AddSpaStaticFiles(z =>
            {
                z.RootPath = "wwwroot/dist";
            });

            services.AddAuthorization(z =>
            {
                z.AddPolicy("VKPolicy", e =>
                {
                    e.RequireClaim("VKToken");
                });
            });

            services.Configure<MailOptions>(Configuration.GetSection("Mail"));
            services.Configure<GoogleRecaptcha>(Configuration.GetSection("GoogleRecaptcha"));
            services.AddTransient<IGoogleRecaptchaService, GoogleRecaptchaService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddHttpClient();
            services.AddScoped<IBaseHttpClient, BaseHttpClient>();
            services.AddScoped<IUserManagerService, UserManagerService>();
            services.Configure<VkontakteOptions>(Configuration.GetSection("Vkontakte"));
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
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseCors(opts =>
            {
                opts.AllowAnyMethod();
                opts.AllowAnyOrigin();
                opts.AllowAnyHeader();
            });
            app.UseMvc(builder =>
            {
                builder.MapRoute("category", "category/{slug}",
                    new { controller = "category", action = "Index" });

                builder.MapRoute("default", "{controller}/{action}/{id?}",
                        new {controller = "Home", action = "Index"});
                });

            app.UseSpaStaticFiles();
            app.Map("/repost/user", builder =>
            {
                builder.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment())
                    {
                        spa.UseAngularCliServer("start");
                    }
                });
            });

            DataBaseInitializer.Init(serviceProvider);
            IdentityInitializer.Init(serviceProvider);
        }
    }
}
