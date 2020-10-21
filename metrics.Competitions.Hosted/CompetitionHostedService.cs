using System.Threading;
using System.Threading.Tasks;
using metrics.BackgroundJobs.Abstractions;
using Microsoft.Extensions.Hosting;

namespace metrics.Competitions.Hosted
{
    public class CompetitionHostedService : BackgroundService
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public CompetitionHostedService(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _backgroundJobService.Register<ICompetitionService>("CompetitionJob",x => x.ExecuteAsync(), "0 0 * ? * *");
            return Task.CompletedTask;
        }
    }
}