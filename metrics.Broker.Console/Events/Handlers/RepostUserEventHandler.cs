using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Console.Services;
using metrics.Broker.Events.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using IBackgroundJobService = metrics.BackgroundJobs.Abstractions.IBackgroundJobService;

namespace metrics.Broker.Console.Events.Handlers
{
    public class RepostUserEventHandler : IMessageHandler<IExecuteNextRepost>
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IBackgroundJobService _jobService;
        private readonly ILogger<RepostUserEventHandler> _logger;

        public RepostUserEventHandler(ITransactionScopeFactory transactionScopeFactory,
            IBackgroundJobService jobService, ILogger<RepostUserEventHandler> logger)
        {
            _transactionScopeFactory = transactionScopeFactory;
            _jobService = jobService;
            _logger = logger;
        }

        public async Task HandleAsync(IExecuteNextRepost obj, CancellationToken token = default)
        {
            try
            {
                using var transaction = await _transactionScopeFactory.CreateAsync(token);

                var message = await transaction
                    .Query<VkRepost>()
                    .OrderBy(f => f.DateStatus)
                    .Where(f => f.Status == VkRepostStatus.New && f.UserId == obj.UserId)
                    .FirstOrDefaultAsync(token);

                if (message == null)
                    return;

                message.Status = VkRepostStatus.Pending;
                message.DateStatus = DateTime.Now;

                await transaction.GetRepository<VkRepost>().UpdateAsync(message, token);

                await transaction.CommitAsync(token);
                    
                _jobService.Schedule<ISchedulerJobService>(
                    client => client.Repost(message.OwnerId, message.MessageId, obj.UserId),
                    TimeSpan.FromSeconds(10));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during consuming {nameof(RepostUserEventHandler)}");
            }
        }
    }
}