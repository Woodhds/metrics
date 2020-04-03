using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Services.Concrete;
using metrics.Services.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace metrics.Handlers
{
    public class RepostEndEventHandler : IMessageHandler<RepostEndEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IRepostCacheAccessor _repostCacheAccessor;

        public RepostEndEventHandler(IHubContext<NotificationHub> hubContext, IRepostCacheAccessor repostCacheAccessor)
        {
            _hubContext = hubContext;
            _repostCacheAccessor = repostCacheAccessor;
        }

        public Task HandleAsync(RepostEndEvent obj, CancellationToken token = default)
        {
            return _hubContext.Clients.User(obj.UserId.ToString())
                .SendAsync("Count", _repostCacheAccessor.GetCountAsync(obj.UserId), token);
        }
    }
}