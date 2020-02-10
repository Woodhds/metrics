using System;
using System.Threading;
using System.Threading.Tasks;
using Base.Abstractions;
using metrics.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;

namespace Competition.Hosted
{
    public class CompetitionService : BackgroundService
    {
        private readonly IElasticClientFactory _elasticClientProvider;
        private readonly IServiceProvider _serviceProvider;

        public CompetitionService(IElasticClientFactory elasticClientProvider, IServiceProvider serviceProvider)
        {
            _elasticClientProvider = elasticClientProvider;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var service in _serviceProvider.GetServices<ICompetitionsService>())
                {
                    var data = await service.Fetch();
                    await _elasticClientProvider.Create().IndexManyAsync(data, cancellationToken: stoppingToken);
                    await Task.Delay(900 * 10, stoppingToken);
                }
                

                await Task.Delay(50000 * 20, stoppingToken);
            }
        }
    }
}