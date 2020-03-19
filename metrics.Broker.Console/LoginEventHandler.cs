using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace metrics.Broker.Console
{
    public class LoginEventHandler : IMessageHandler<LoginEvent>
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public LoginEventHandler(ITransactionScopeFactory transactionScopeFactory)
        {
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task HandleAsync([NotNull] LoginEvent obj, CancellationToken token = default)
        {
            var ctx = await _transactionScopeFactory.CreateAsync(cancellationToken: token);
            var repository = ctx.GetRepository<UserToken>();
            var userT = new UserToken {Token = obj.Token, UserId = obj.UserId};
            if (await repository.Read().AnyAsync(userToken => userToken.UserId == obj.UserId, token))
            {
                await repository.UpdateAsync(userT);
            }
            else
            {
                await repository.CreateAsync(userT);
            }

            await ctx.CommitAsync(token);
        }
    }
}