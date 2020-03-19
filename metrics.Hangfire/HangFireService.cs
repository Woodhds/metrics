using System;
using System.Collections.Generic;
using System.Linq;
using Base.Contracts;
using Hangfire;
using metrics.Hangfire.Abstractions;
using metrics.Services.Abstractions;

namespace metrics.Hangfire
{
    public class HangFireService : IHangfireService
    {
        private readonly IBackgroundJobClient _backgroundJobService;

        public HangFireService(IBackgroundJobClient backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public void RegisterRepost(IEnumerable<VkRepostViewModel> reposts, int timeout = 60)
        {
            
        }
    }
}