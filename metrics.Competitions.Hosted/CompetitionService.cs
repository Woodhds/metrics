using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Abstractions;
using metrics.Competitions.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;

namespace metrics.Competitions.Hosted
{
    public class CompetitionService : BackgroundService
    {
        private readonly IElasticClientFactory _elasticClientProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CompetitionService> _logger;

        public CompetitionService(
            IElasticClientFactory elasticClientProvider,
            IServiceProvider serviceProvider,
            ILogger<CompetitionService> logger
        )
        {
            _elasticClientProvider = elasticClientProvider;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var service in _serviceProvider.GetServices<ICompetitionsService>())
                {
                    try
                    {
                        Console.WriteLine($"Start fetching from \"{service.GetType().Name}\" {DateTimeOffset.Now}");
                        var data = await service.Fetch();
                        if (!data.Any()) continue;

                        await _elasticClientProvider.Create().IndexManyAsync(data, cancellationToken: stoppingToken);
                        await Task.Delay(900 * 10, stoppingToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error fetch posts");
                    }
                    finally
                    {
                        Console.WriteLine($"Stop fetching from \"{service.GetType().Name}\" {DateTimeOffset.Now}");
                    }
                }


                await Task.Delay(50000 * 20, stoppingToken);
            }
        }
    }
}