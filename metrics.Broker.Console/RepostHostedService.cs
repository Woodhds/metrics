using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Services.Abstractions;
using Microsoft.Extensions.Hosting;

namespace metrics.Broker.Console
{
    public class RepostHostedService : BackgroundService
    {
        private readonly IVkClient _vkClient;
        private readonly IRepostCacheAccessor _repostCacheAccessor;

        public RepostHostedService(IVkClient vkClient, IRepostCacheAccessor repostCacheAccessor)
        {
            _vkClient = vkClient;
            _repostCacheAccessor = repostCacheAccessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var reposts = _repostCacheAccessor.Get();
                foreach (var repost in reposts)
                {
                    if (repost.repost != null)
                    {
                        await _vkClient.Repost(new List<VkRepostViewModel> {repost.repost}, 1, repost.userId);
                    }
                }

                await Task.Delay(30 * 1000, stoppingToken);
            }
        }
    }
}