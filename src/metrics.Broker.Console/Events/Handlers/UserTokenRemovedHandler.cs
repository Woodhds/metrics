using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using Base.Contracts.Events;
using metrics.Broker.Abstractions;
using metrics.Cache.Abstractions;
using metrics.Identity.Client.Abstractions;

namespace metrics.Broker.Console.Events.Handlers
{
    public class UserTokenRemovedHandler : IMessageHandler<UserTokenRemoved>
    {
        private readonly ICachingService _cachingService;
        private readonly IUserTokenKeyProvider _userTokenKeyProvider;

        public UserTokenRemovedHandler(ICachingService cachingService, IUserTokenKeyProvider userTokenKeyProvider)
        {
            _cachingService = cachingService;
            _userTokenKeyProvider = userTokenKeyProvider;
        }

        public Task HandleAsync(UserTokenRemoved obj, CancellationToken token = default)
        {
            return _cachingService.RemoveAsync(_userTokenKeyProvider.GetKey(obj.UserId), token);
        }
    }
}