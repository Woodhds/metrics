using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Cache.Abstractions;

namespace metrics.Broker.Console
{
    public class RepostedEventHandler : IMessageHandler<RepostedEvent>
    {
        private readonly ICachingService _cachingService;
        private readonly IMessageBroker _messageBroker;

        public RepostedEventHandler(ICachingService cachingService, IMessageBroker messageBroker)
        {
            _cachingService = cachingService;
            _messageBroker = messageBroker;
        }

        public async Task HandleAsync([NotNull] RepostedEvent obj, CancellationToken token = default)
        {
            if (obj.UserId == default)
                return;

            var list = await _cachingService.GetAsync<List<VkRepostViewModel>>(obj.UserId.ToString(), token);
            if (list == null || list.Count == 0)
            {
                return;
            }

            var message = list.FirstOrDefault(z => z.Owner_Id == obj.OwnerId && z.Id == obj.Id);
            if (message != null)
            {
                list.Remove(message);
            }

            if (list.Count == 0)
            {
                await _cachingService.RemoveAsync(obj.UserId.ToString(), token);
                var queue = await _cachingService.GetAsync<List<int>>("queue", token);
                await _cachingService.SetAsync("queue", queue.Where(f => f != obj.UserId), token);
            }
            else
            {
                await _cachingService.SetAsync(obj.UserId.ToString(), list, token);
            }

            await _messageBroker.PublishAsync(new RepostEndEvent {UserId = obj.UserId}, token);
        }
    }
}