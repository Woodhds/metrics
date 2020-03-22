using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;

namespace metrics.Broker.Console
{
    public class RepostEventGroupCreatedHandler : IMessageHandler<RepostGroupCreatedEvent>
    {
        private readonly IRepostCacheAccessor _repostCacheAccessor;

        public RepostEventGroupCreatedHandler(IRepostCacheAccessor repostCacheAccessor)
        {
            _repostCacheAccessor = repostCacheAccessor;
        }

        public Task HandleAsync(RepostGroupCreatedEvent obj, CancellationToken token = default)
        {
            _repostCacheAccessor.SetAsync(obj.UserId, obj.Reposts);
            return Task.CompletedTask;
        }
    }
}