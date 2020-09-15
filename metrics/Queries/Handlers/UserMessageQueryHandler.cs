using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Authentication.Infrastructure;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.EventSourcing.Abstractions.Query;
using metrics.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace metrics.Queries.Handlers
{
    public class UserMessageQueryHandler : IQueryHandler<UserMessageQuery, DataSourceResponseModel>
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IAuthenticatedUserProvider _authenticatedUserProvider;
        private readonly IVkClient _vkClient;

        public UserMessageQueryHandler(
            ITransactionScopeFactory transactionScopeFactory,
            IAuthenticatedUserProvider authenticatedUserProvider,
            IVkClient vkClient
        )
        {
            _transactionScopeFactory = transactionScopeFactory;
            _authenticatedUserProvider = authenticatedUserProvider;
            _vkClient = vkClient;
        }

        public async Task<DataSourceResponseModel> ExecuteAsync(UserMessageQuery messageQuery,
            CancellationToken token = default)
        {
            var (page, pageSize) = messageQuery;
            using var scope = await _transactionScopeFactory.CreateAsync(token);

            var query = scope.Query<VkRepost>()
                .Where(f => f.UserId == _authenticatedUserProvider.GetUser().Id).OrderByDescending(f => f.Id);

            var data = await query.Skip(page * pageSize).Take(pageSize).ToListAsync(token);
            var posts = await _vkClient.GetById(
                data.Select(f => new VkRepostViewModel(f.OwnerId, f.MessageId)));
            var texts = posts
                .Response.Items
                .Select(f => new
                {
                    f.Text, f.Id,
                    DateStatus = data.Where(a => a.OwnerId == f.Owner_Id && a.MessageId == f.Id)
                        .Select(a => a.DateStatus)
                });

            return new DataSourceResponseModel(texts, await query.CountAsync(token));
        }
    }
}