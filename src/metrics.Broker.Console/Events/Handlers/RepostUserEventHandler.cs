﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Console.Services;
using metrics.Broker.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using IBackgroundJobService = metrics.BackgroundJobs.Abstractions.IBackgroundJobService;

namespace metrics.Broker.Console.Events.Handlers
{
    public class RepostUserEventHandler : IMessageHandler<ExecuteNextRepost>
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IBackgroundJobService _jobService;
        private readonly ILogger<RepostUserEventHandler> _logger;
        private readonly IMessageBroker _messageBroker;

        public RepostUserEventHandler(
            ITransactionScopeFactory transactionScopeFactory,
            IBackgroundJobService jobService,
            ILogger<RepostUserEventHandler> logger, IMessageBroker messageBroker)
        {
            _transactionScopeFactory = transactionScopeFactory;
            _jobService = jobService;
            _logger = logger;
            _messageBroker = messageBroker;
        }

        public async Task HandleAsync(ExecuteNextRepost obj, CancellationToken token = default)
        {
            try
            {
                await using var transaction = await _transactionScopeFactory.CreateAsync(token);

                var message = await transaction
                    .Query<VkRepost>()
                    .OrderBy(f => f.DateStatus)
                    .Where(f => f.Status == VkRepostStatus.New && f.UserId == obj.UserId)
                    .TagWith("Select next repost message")
                    .Select(x => new VkRepost
                    {
                        Id = x.Id, 
                        RowVersion = x.RowVersion,
                        OwnerId = x.OwnerId,
                        MessageId = x.MessageId
                    })
                    .FirstOrDefaultAsync(token);

                if (message == null)
                    return;

                message.Status = VkRepostStatus.Pending;
                message.DateStatus = DateTime.Now;

                await transaction.GetRepository<VkRepost>()
                    .UpdateProperties(message, repost => new {repost.Status, repost.DateStatus}, token);

                await transaction.CommitAsync(token)!;

                _jobService.Schedule<ISchedulerJobService>(
                    client => client.Repost(message.OwnerId, message.MessageId, obj.UserId),
                    TimeSpan.FromSeconds(10));
            }
            catch (Exception e)
            {
                await _messageBroker.SendAsync(new ExecuteNextRepost {UserId = obj.UserId}, token);
                _logger.LogError(e, $"Error during consuming {nameof(RepostUserEventHandler)}");
            }
        }
    }
}