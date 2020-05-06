using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;

namespace metrics.Broker.Console
{
    public class RepostedEventHandler : IMessageHandler<RepostedEvent>
    {
        private readonly IMessageBroker _messageBroker;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public RepostedEventHandler(IMessageBroker messageBroker, ITransactionScopeFactory transactionScopeFactory)
        {
            _messageBroker = messageBroker;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task HandleAsync([NotNull] RepostedEvent obj, CancellationToken token = default)
        {
            if (obj.UserId == default)
                return;

            using var scope = await _transactionScopeFactory.CreateAsync(cancellationToken: token);

            var message = scope.GetRepository<VkRepost>()
                .Read()
                .FirstOrDefault(q =>
                    q.UserId == obj.UserId && 
                    obj.OwnerId == q.OwnerId && 
                    q.Status == VkRepostStatus.Pending &&
                    q.MessageId == obj.Id
                );

            if (message == null)
            {
                return;
            }

            message.Status = VkRepostStatus.Complete;
            message.DateStatus = DateTime.Now;

            await scope.GetRepository<VkRepost>().UpdateAsync(message);

            await _messageBroker.PublishAsync(new NotifyUserEvent {UserId = obj.UserId}, token);
            await scope.CommitAsync(token);
        }
    }
}