using System.Threading.Tasks;
using metrics.Services.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace metrics.Notification.SignalR.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IRepostCacheAccessor _repostCacheAccessor;

        public NotificationHub(IRepostCacheAccessor repostCacheAccessor)
        {
            _repostCacheAccessor = repostCacheAccessor;
        }

        [HubMethodName("currentCount")]
        public async Task CurrentCount()
        {
            await Clients.User(Context.UserIdentifier)
                .SendAsync("Count", await _repostCacheAccessor.GetCountAsync(int.Parse(Context.UserIdentifier)));
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await CurrentCount();
        }
    }
}