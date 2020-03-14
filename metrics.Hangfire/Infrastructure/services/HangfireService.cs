using System;
using Hangfire;
using metrics.Hangfire.Abstractions;

namespace metrics.Hangfire.Infrastructure.services
{
    public class HangfireService : IHangfireService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        
        public void RegisterRepost(int id, int ownerId, TimeSpan delay)
        {
            
        }

        public void RegisterJoin(int id, TimeSpan delay)
        {
            throw new NotImplementedException();
        }
    }
}