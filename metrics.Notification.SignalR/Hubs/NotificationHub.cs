using System.Threading.Tasks;
using metrics.Services.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace metrics.Notification.SignalR.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IUserRepostedService _userRepostedService;

        public NotificationHub(IUserRepostedService repostCacheAccessor)
        {
            _userRepostedService = repostCacheAccessor;
        }

        [HubMethodName("currentCount")]
        public async Task CurrentCount()
        {
            await Clients.User(Context.UserIdentifier)
                .SendAsync("Count", await _userRepostedService.GetCountAsync(int.Parse(Context.UserIdentifier)));
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await CurrentCount();
        }
    }
}