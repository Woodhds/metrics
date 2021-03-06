﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace metrics.Broker.Console.Events.Handlers
{
    public class RepostedEventHandler : IMessageHandler<RepostCreated>
    {
        private readonly IMessageBroker _messageBroker;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public RepostedEventHandler(IMessageBroker messageBroker, ITransactionScopeFactory transactionScopeFactory)
        {
            _messageBroker = messageBroker;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task HandleAsync([NotNull] RepostCreated obj, CancellationToken token = default)
        {
            if (obj.UserId == default)
                return;

            await using var scope = await _transactionScopeFactory.CreateAsync(token);

            var message = await scope.Query<VkRepost>()
                .Where(q =>
                    q.UserId == obj.UserId &&
                    obj.OwnerId == q.OwnerId &&
                    q.Status == VkRepostStatus.Pending || q.Status == VkRepostStatus.New &&
                    q.MessageId == obj.Id
                )
                .Select(a => new VkRepost {Id = a.Id, RowVersion = a.RowVersion})
                .FirstOrDefaultAsync(token);

            if (message == null)
            {
                return;
            }

            message.Status = VkRepostStatus.Complete;
            message.DateStatus = DateTime.Now;

            await scope.GetRepository<VkRepost>().UpdateProperties(message,
                repost => new {repost.Status, repost.DateStatus}, CancellationToken.None);

            await scope.CommitAsync(token);

            await Task.WhenAll(
                _messageBroker.PublishAsync(
                    new NotifyUserEvent
                    {
                        UserId = obj.UserId,
                        MessageId = message.MessageId,
                        OwnerId = message.OwnerId
                    },
                    token),
                _messageBroker.SendAsync(new ExecuteNextRepost
                {
                    UserId = obj.UserId
                }, token)
            );
        }
    }
}