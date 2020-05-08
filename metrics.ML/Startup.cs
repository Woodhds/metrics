using Base.Abstractions;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Confguraton;
using metrics.Data.Sql;
using metrics.Data.Sql.Extensions;
using metrics.ML.Services;
using metrics.ML.Services.Abstractions;
using metrics.ML.Services.Extensions;
using metrics.ML.Services.Services;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace metrics.ML
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
            services.AddGrpc(); 
            //services.AddHostedService<VkMessageMLService>();
            services.AddSingleton<IEntityConfiguration, RepostEntityConfiguration>();
            services.AddDataContext<DataContext>(_configuration.GetConnectionString("DataContext"));
            services.AddElastic(_configuration);
            services.AddPredictClient("http://localhost:5005");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<MessagePredictService>();
            });
        }
    }
}