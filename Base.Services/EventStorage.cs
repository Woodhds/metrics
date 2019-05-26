using System.Collections.Concurrent;
using System.Threading.Tasks;
using Base.Abstract;
using Base.Extensions;
using Microsoft.AspNetCore.Http;

namespace Base.Services
{
    public class EventStorage: IEventStorage
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string UserId => _httpContextAccessor.HttpContext.User.Identity.GetId();

        private ConcurrentDictionary<string, Deque<IRequest>> _queue =
            new ConcurrentDictionary<string, Deque<IRequest>>();

        public EventStorage(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddEvent(IRequest request)
        {
            if (!_queue.ContainsKey(UserId))
            {
                _queue.TryAdd(UserId, new Deque<IRequest>());
            }
            _queue[UserId].AddLast(request);
        }
    }
}