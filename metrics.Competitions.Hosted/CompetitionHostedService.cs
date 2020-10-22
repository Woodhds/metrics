using System.Threading;
using System.Threading.Tasks;
using Hangfire;
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
            _backgroundJobService.Register<ICompetitionService>("CompetitionJob",
                x => x.ExecuteAsync(CancellationToken.None), Cron.Hourly(), "competition");
            return Task.CompletedTask;
        }
    }
}