﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using IBackgroundJobService = metrics.BackgroundJobs.Abstractions.IBackgroundJobService;

namespace metrics.Broker.Console
{
    public class RepostUserEventHandler : IMessageHandler<ExecuteNextRepost>
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

        public async Task HandleAsync(ExecuteNextRepost obj, CancellationToken token = default)
        {
            try
            {
                var scope = await _transactionScopeFactory.CreateAsync(cancellationToken: token);
                var message = await scope
                    .GetRepository<VkRepost>()
                    .Read()
                    .OrderBy(f => f.DateStatus)
                    .Where(f => f.Status == VkRepostStatus.New && f.UserId == obj.UserId)
                    .FirstOrDefaultAsync(token);

                if (message == null)
                    return;

                message.Status = VkRepostStatus.Pending;
                message.DateStatus = DateTime.Now;

                await scope.GetRepository<VkRepost>().UpdateAsync(message);

                _jobService.Schedule<IVkClient>(
                    client => client.Repost(message.OwnerId, message.MessageId, 1, obj.UserId),
                    TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during consuming {nameof(RepostUserEventHandler)}");
            }
        }
    }
}