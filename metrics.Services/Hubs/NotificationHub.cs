using System;
using System.Threading.Tasks;
using metrics.Services.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace metrics.Services.Hubs
{
    [Authorize(Policy = "VkPolicy")]
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
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await base.OnConnectedAsync();
            await CurrentCount();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await base.OnDisconnectedAsync(exception);
        }
    }
}