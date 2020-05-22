using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Cache.Abstractions;

namespace metrics.Broker.Console
{
    public class LoginEventHandler : IMessageHandler<LoginEvent>
    {
        private readonly ICachingService _cachingService;

        public LoginEventHandler(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        public Task HandleAsync([NotNull] LoginEvent obj, CancellationToken token = default)
        {
            return _cachingService.SetAsync(obj.UserId.ToString(), obj.Token, token);
        }
    }
}