using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.Events;

namespace metrics.Handlers
{
    public class SetTypeEventHandler : IMessageHandler<SetMessageTypeEvent>
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public SetTypeEventHandler(ITransactionScopeFactory transactionScopeFactory)
        {
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task HandleAsync(SetMessageTypeEvent obj, CancellationToken token = default)
        {
            var scope = await _transactionScopeFactory.CreateAsync(cancellationToken: token);
            var message = scope.GetRepository<MessageVk>().Read()
                .FirstOrDefault(a => a.MessageId == obj.MessageId && a.OwnerId == obj.OwnerId);
            if (message == null)
                return;
            
            message.MessageCategoryId = obj.MessageCategory;

            await scope.GetRepository<MessageVk>().UpdateAsync(message);
            await scope.CommitAsync(token);
        }
    }
}