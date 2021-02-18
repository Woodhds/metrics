using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Events;
using metrics.Notification.SignalR.Hubs;
using metrics.Services.Concrete;
using Microsoft.AspNetCore.SignalR;

namespace metrics.Handlers
{
    public class NotifyUserEventHandler : IMessageHandler<NotifyUserEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUserRepostedService _repostCacheAccessor;

        public NotifyUserEventHandler(
            IHubContext<NotificationHub> hubContext,
            IUserRepostedService repostCacheAccessor
        )
        {
            _hubContext = hubContext;
            _repostCacheAccessor = repostCacheAccessor;
        }

        public async Task HandleAsync(NotifyUserEvent obj, CancellationToken token = default)
        {
            await _hubContext.Clients.User(obj.UserId.ToString())
                .SendAsync("Count", await _repostCacheAccessor.GetCountAsync(obj.UserId), token);

            if (obj.MessageId.HasValue && obj.OwnerId.HasValue)
            {
                await _hubContext.Clients.User(obj.UserId.ToString())
                    .SendAsync("reposted", new {obj.MessageId, obj.OwnerId}, token);
            }
        }
    }
}