using Data.EF;
using DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Data.Entities;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore.Design;

namespace metrics
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var connectionString = Configuration.GetConnectionString("DataContext");
            services.AddDbContext<DataContext>(opts => { opts.UseNpgsql(connectionString);});
            services.AddScoped<DbContext, DataContext>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).ConfigureApplicationPartManager(
                manager =>
                {
                    manager.FeatureProviders.Add(new GenericControllerFeatureProvider());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvcWithDefaultRoute();

            app.Map("/admin", builder =>
            {
                builder.UseMvc(routeBuilder => { routeBuilder.MapRoute("admin", "{controller}/{action}/{id?}"); });
                builder.UseSpa(spaBuilder =>
                {
                    spaBuilder.Options.SourcePath = "scripts/app";
                    spaBuilder.UseSpaPrerendering(options =>
                    {
                        options.BootModulePath = $"{spaBuilder.Options.SourcePath}/dist-server/main.bundle.js";
                        options.BootModuleBuilder = env.IsDevelopment()
                            ? new AngularCliBuilder(npmScript: "build:ssr")
                            : null;
                        options.ExcludeUrls = new[] { "/sockjs-node" };
                    });
                    if (env.IsDevelopment())
                    {
                        spaBuilder.UseAngularCliServer(npmScript: "start");
                    }
                });
            });
        }
    }
}
