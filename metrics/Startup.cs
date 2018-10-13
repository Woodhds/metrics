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

            services.AddAuthentication()
                .AddFacebook(z => {
                    z.AppId = Configuration.GetValue<string>("Facebook:AppId");
                    z.ClientSecret = Configuration.GetValue<string>("Facebook:ClientSecret");
                })

                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                {
                    opts.Audience = "metrics";
                    opts.ClaimsIssuer = "metrics";
                });

            services.ConfigureApplicationCookie(z =>
            {
                z.LoginPath = "/Account/Login";
                z.ReturnUrlParameter = "returnUrl";
                z.Cookie.HttpOnly = true;
            });

            
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).ConfigureApplicationPartManager(
                manager => { manager.FeatureProviders.Add(new GenericControllerFeatureProvider()); });
            services.AddSpaStaticFiles(z =>
            {
                z.RootPath = "ClientApp/dist";
            });

            services.Configure<MailOptions>(Configuration.GetSection("Mail"));
            services.AddScoped<IEmailService, EmailService>();
            services.AddHttpClient();
            services.AddScoped<IHttpClient, BaseHttpClient>();
            services.AddScoped<IUserManagerService, UserManagerService>();
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
            

            app.Map("/admin", builder =>
            {
                builder.UseSpaStaticFiles();
                builder.UseSpa(spaBuilder =>
                {
                    spaBuilder.Options.SourcePath = "ClientApp";
                    if (env.IsDevelopment())
                    {
                        spaBuilder.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                    }
                });
            });

            IdentityInitializer.Init(serviceProvider);
        }
    }
}
