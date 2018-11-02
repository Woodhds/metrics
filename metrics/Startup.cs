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
using metrics.Services.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NSwag.AspNetCore;
using DAL.Services.Abstract;
using Core.Services.Concrete;
using Newtonsoft.Json;
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

            services.Configure<JwtOptions>(Configuration.GetSection("Jwt"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                })
                .AddCookie();

            services.ConfigureApplicationCookie(z =>
            {
                z.LoginPath = "/Account/Login";
                z.ReturnUrlParameter = "returnUrl";
                z.Cookie.HttpOnly = true;
                z.SlidingExpiration = true;
                z.ExpireTimeSpan = TimeSpan.FromDays(1);
            });


            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).ConfigureApplicationPartManager(
                manager => { manager.FeatureProviders.Add(new GenericControllerFeatureProvider()); }).AddJsonOptions(
                z =>
                {
                    z.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            services.AddAuthorization(z =>
            {
                z.AddPolicy("VKPolicy", e =>
                {
                    e.RequireClaim(Constants.VK_TOKEN_CLAIM);
                });
            });

            services.Configure<MailOptions>(Configuration.GetSection("Mail"));
            services.Configure<GoogleRecaptcha>(Configuration.GetSection("GoogleRecaptcha"));
            services.AddTransient<IGoogleRecaptchaService, GoogleRecaptchaService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddHttpClient();
            services.AddLogging();
            services.AddScoped<IBaseHttpClient, BaseHttpClient>();
            services.AddScoped<IUserManagerService, UserManagerService>();
            services.Configure<VkontakteOptions>(Configuration.GetSection("Vkontakte"));
            services.Configure<VKApiUrls>(Configuration.GetSection("VKApiUrls"));
            services.AddSingleton<IVkClient, VkClient>();
            services.AddSingleton<IViewConfigService, ViewConfigService>();
            services.AddSwagger();
            services.AddSpaStaticFiles(e =>
            {
                e.RootPath = "dist";
            });
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

            app.UseSpaStaticFiles();
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
                builder.MapRoute("default", "{controller}/{action}/{id?}",
                    new {controller = "Home", action = "Index"});
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200/");
                }
            });

            app.UseSwaggerUi3WithApiExplorer(settings =>
            {
                settings.GeneratorSettings.DefaultPropertyNameHandling = NJsonSchema.PropertyNameHandling.CamelCase;
            });

            DataBaseInitializer.Init(serviceProvider);
            IdentityInitializer.Init(serviceProvider);
            ViewConfigInitializer.Init(serviceProvider);
        }
    }
}
