using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Abstractions;
using metrics.Services.Abstract;
using Microsoft.Extensions.Hosting;
using Nest;

namespace Competition.Hosted
{
    public class CompetitionService : BackgroundService
    {
        private readonly IElasticClientProvider _elasticClientProvider;
        private readonly ICompetitionsService _competitionsService;

        public CompetitionService(IElasticClientProvider elasticClientProvider,
            ICompetitionsService competitionsService)
        {
            _elasticClientProvider = elasticClientProvider;
            _competitionsService = competitionsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var data = await _competitionsService.Fetch();
                await _elasticClientProvider.GetClient().IndexManyAsync(data, cancellationToken: stoppingToken);

                await Task.Delay(50000 * 20, stoppingToken);
            }
        }
    }
}