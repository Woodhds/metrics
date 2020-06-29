using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using Microsoft.Extensions.Hosting;

namespace metrics.Broker.Console
{
    public class RepostHostedService : BackgroundService
    {
        private readonly IRepostCacheAccessor _repostCacheAccessor;
        private readonly BackgroundJobs.Abstractions.IBackgroundJobService _backgroundJobService;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public RepostHostedService(
            IRepostCacheAccessor repostCacheAccessor,
            BackgroundJobs.Abstractions.IBackgroundJobService backgroundJobService,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _repostCacheAccessor = repostCacheAccessor;
            _backgroundJobService = backgroundJobService;
            _transactionScopeFactory = transactionScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var reposts = await _repostCacheAccessor.GetAsync(stoppingToken);
                if (reposts != null)
                {
                    var userGroups = reposts.GroupBy(f => new {f.userId, f.last}).ToArray();
                    foreach (var t in userGroups)
                    {
                        var startDate = t.Select(f => f.last).FirstOrDefault();
                        if (startDate < DateTime.Now)
                        {
                            startDate = DateTime.Now;
                        }

                        var userReposts = t.ToArray();

                        for (var i = 0; i < userReposts.Length; i++)
                        {
                            startDate = startDate.Add(TimeSpan.FromSeconds((i + 1) * 30));
                            System.Console.WriteLine("SCHEDULE AT: " + startDate);
                            _backgroundJobService.Schedule<IVkClient>(v => v.Repost(userReposts[i].repost.Owner_Id,
                                userReposts[i].repost.Id, 1, t.Key.userId), startDate);
                        }

                        using var scope = await _transactionScopeFactory.CreateAsync(cancellationToken: stoppingToken);
                        
                        var repository = scope.GetRepository<VkRepostUserOffset>();

                        if (repository.Read().Any(f => f.UserId == t.Key.userId))
                        {
                            await repository.UpdateAsync(new VkRepostUserOffset
                                {LastPost = startDate, UserId = t.Key.userId}, stoppingToken);
                        }
                        else
                        {
                            await repository.CreateAsync(new VkRepostUserOffset
                                {LastPost = startDate, UserId = t.Key.userId}, stoppingToken);
                        }

                        await scope.CommitAsync(stoppingToken);
                    }
                }

                await Task.Delay(30 * 1000, stoppingToken);
            }
        }
    }
}