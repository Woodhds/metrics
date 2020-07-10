﻿using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Services.Concrete;

namespace metrics.Broker.Console
{
    public class RepostEventGroupCreatedHandler : IMessageHandler<CreateRepostGroup>
    {
        private readonly IUserRepostedService _repostCacheAccessor;
        private readonly IMessageBroker _messageBroker;

        public RepostEventGroupCreatedHandler(IUserRepostedService repostCacheAccessor, IMessageBroker messageBroker)
        {
            _repostCacheAccessor = repostCacheAccessor;
            _messageBroker = messageBroker;
        }

        public async Task HandleAsync(CreateRepostGroup obj, CancellationToken token = default)
        {
            await _repostCacheAccessor.SetAsync(obj.UserId, obj.Reposts);
            await _messageBroker.PublishAsync(new NotifyUserEvent {UserId = obj.UserId}, token);
            await _messageBroker.SendAsync(new ExecuteNextRepost {UserId = obj.UserId}, token);
        }
    }
}