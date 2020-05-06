using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Services.Concrete;

namespace metrics.Broker.Console
{
    public class RepostEventGroupCreatedHandler : IMessageHandler<RepostGroupCreatedEvent>
    {
        private readonly IRepostCacheAccessor _repostCacheAccessor;
        private readonly IMessageBroker _messageBroker;

        public RepostEventGroupCreatedHandler(IRepostCacheAccessor repostCacheAccessor, IMessageBroker messageBroker)
        {
            _repostCacheAccessor = repostCacheAccessor;
            _messageBroker = messageBroker;
        }

        public async Task HandleAsync(RepostGroupCreatedEvent obj, CancellationToken token = default)
        {
            await _repostCacheAccessor.SetAsync(obj.UserId, obj.Reposts);
            await _messageBroker.PublishAsync(new NotifyUserEvent {UserId = obj.UserId}, token);
        }
    }
}