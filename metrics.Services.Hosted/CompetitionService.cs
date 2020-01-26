using System.Threading;
using System.Threading.Tasks;
using metrics.Services.Abstract;
using Microsoft.Extensions.Hosting;

namespace metrics.Services.Hosted
{
    public class CompetitionService : IHostedService
    {
        private readonly ICompetitionsService _competitionsService;
        
        public CompetitionService(ICompetitionsService competitionsService)
        {
            _competitionsService = competitionsService;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}