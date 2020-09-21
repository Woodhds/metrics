using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Cache.Abstractions;

namespace metrics.Broker.Console.Events.Handlers
{
    public class LoginEventHandler : IMessageHandler<ILoginEvent>
    {
        private readonly ICachingService _cachingService;

        public LoginEventHandler(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        public Task HandleAsync([NotNull] ILoginEvent obj, CancellationToken token = default)
        {
            return _cachingService.SetAsync(obj.UserId.ToString(), obj.Token, TimeSpan.FromHours(6), token);
        }
    }
}