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
            var scope = await _transactionScopeFactory.CreateResilientAsync(token);

            await scope.ExecuteAsync(async transaction =>
            {
                var message = transaction.Query<MessageVk>()
                    .FirstOrDefault(a => a.MessageId == obj.MessageId && a.OwnerId == obj.OwnerId);
                if (message == null)
                {
                    await transaction.GetRepository<MessageVk>().CreateAsync(new MessageVk
                    {
                        MessageId = obj.MessageId,
                        OwnerId = obj.OwnerId,
                        MessageCategoryId = obj.MessageCategory
                    }, token);
                }
                else
                {
                    message.MessageCategoryId = obj.MessageCategory;

                    await transaction.GetRepository<MessageVk>().UpdateAsync(message, token);
                }


                await transaction.CommitAsync(token);
            });
        }
    }
}