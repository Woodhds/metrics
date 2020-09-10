using System.Threading;
using System.Threading.Tasks;
using metrics.BackgroundJobs.Abstractions;
using metrics.Broker.Console.Services;
using Microsoft.Extensions.Hosting;

namespace metrics.Broker.Console
{
    public class StartupService : BackgroundService
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public StartupService(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _backgroundJobService.Execute<IRandomLikeService>(x => x.ExecuteRandomLike(0));
            return Task.CompletedTask;
        }
    }
}