﻿using Elastic.Client;
using metrics.Broker.Abstractions;
using metrics.Data.Abstractions;
using metrics.Data.Common;
using metrics.Data.Common.Infrastructure.Configuration;
using metrics.Data.Sql;
using metrics.ML.Services;
using metrics.ML.Services.Extensions;
using metrics.ML.Services.Services;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Services.Extensions;
using metrics.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace metrics.ML
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<MessagePredictService>();
            });
        }

        protected override void AddBrokerHandlers(IMessageHandlerProvider provider)
        {
           
        }

        protected override void ConfigureApplicationServices(IServiceCollection services)
        {
            services.AddGrpc();
            //services.AddHostedService<VkMessageMLService>();
            //services.AddHostedService<TransformMessageService>();
            services.AddSingleton<IEntityConfiguration, RepostEntityConfiguration>();
            services.AddDataContext<DataContext>(Configuration.GetConnectionString("DataContext"));
            services.AddElastic(Configuration);
            services.AddPredictClient(Configuration["ClientUrl"]);
            services.AddVkClient(Configuration);
            services.AddVkClientConsole(Configuration);
        }

        protected override void ConfigureDataContext(IServiceCollection services)
        {
            
        }
    }
}