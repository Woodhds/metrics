﻿using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Broker.Abstractions;
using metrics.Cache.Abstractions;
using metrics.Identity.Client.Abstractions;

namespace metrics.Broker.Console.Events.Handlers
{
    public class UserTokenChangedHandler : IMessageHandler<IUserTokenChanged>
    {
        private readonly ICachingService _cachingService;
        private readonly IUserTokenKeyProvider _userTokenKeyProvider;

        public UserTokenChangedHandler(ICachingService cachingService, IUserTokenKeyProvider userTokenKeyProvider)
        {
            _cachingService = cachingService;
            _userTokenKeyProvider = userTokenKeyProvider;
        }

        public Task HandleAsync(IUserTokenChanged obj, CancellationToken token = default)
        {
            return _cachingService.SetAsync(_userTokenKeyProvider.GetKey(obj.UserId), obj.Token, token);
        }
    }
}