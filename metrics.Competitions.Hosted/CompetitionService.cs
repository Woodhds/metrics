using System;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Client;
using Hangfire;
using metrics.Competitions.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;

namespace metrics.Competitions.Hosted
{
    public interface ICompetitionService
    {
        Task ExecuteAsync();
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

        [Queue("competition")]
        public async Task ExecuteAsync()
        {
            foreach (var service in _serviceProvider.GetServices<ICompetitionsService>())
            {
                try
                {
                    Console.WriteLine($"Start fetching from \"{service.GetType().Name}\" {DateTimeOffset.Now}");
                    var data = await service.Fetch();
                    if (!data.Any()) continue;

                    var indexingResult = await _elasticClientProvider.Create()
                        .IndexManyAsync(data);
                    await Console.Out.WriteLineAsync($"Indexing result: {indexingResult.IsValid}");
                    await Task.Delay(900 * 10);
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