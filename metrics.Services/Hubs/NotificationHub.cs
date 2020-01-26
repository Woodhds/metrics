using System.Threading.Tasks;
using metrics.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace metrics.Services.Hubs
{
    [Authorize(Policy = "VkPolicy")]
    public class NotificationHub : Hub
    {
        private IEventStorage _eventStorage;

        public NotificationHub(IEventStorage eventStorage)
        {
            _eventStorage = eventStorage;
        }

        [HubMethodName("currentCount")]
        public void CurrentCount()
        {
            Clients.User(Context.UserIdentifier)
                .SendAsync("Count", _eventStorage.GetCurrentCount(Context.UserIdentifier));
        }

        public override Task OnConnectedAsync()
        {
            CurrentCount();
            return base.OnConnectedAsync();
        }
    }
}