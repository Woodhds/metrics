using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Client;
using metrics.Competitions.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;

namespace metrics.Competitions.Hosted
{
    public interface ICompetitionService
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }

    public class CompetitionService : ICompetitionService
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

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            foreach (var service in _serviceProvider.GetServices<ICompetitionsService>())
            {
                try
                {
                    Console.WriteLine($"Start fetching from \"{service.GetType().Name}\" {DateTimeOffset.Now}");
                    var data = await service.Fetch(cancellationToken: cancellationToken);
                    if (!data.Any()) continue;

                    var indexingResult = await _elasticClientProvider.Create()
                        .IndexManyAsync(data, cancellationToken: cancellationToken);
                    await Console.Out.WriteLineAsync($"Indexing result: {indexingResult.IsValid}");
                    await Task.Delay(900 * 10, cancellationToken);
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
        }
    }
}