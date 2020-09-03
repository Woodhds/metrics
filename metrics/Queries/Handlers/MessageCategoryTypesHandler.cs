using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.EventSourcing.Abstractions.Query;
using Microsoft.EntityFrameworkCore;

namespace metrics.Queries.Handlers
{
    public class MessageCategoryTypesHandler : IQueryHandler<MessageCategoryTypesQuery, DataSourceResponseModel>
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public MessageCategoryTypesHandler(ITransactionScopeFactory transactionScopeFactory)
        {
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task<DataSourceResponseModel> ExecuteAsync(MessageCategoryTypesQuery query,
            CancellationToken token = default)
        {
            using var scope = _transactionScopeFactory.CreateQuery();
            var q = scope.Query<MessageCategory>().OrderBy(a => a.Id);
            var (page, pageSize) = query;
            return new DataSourceResponseModel(
                await q.Skip(page * pageSize).Take(pageSize).ToListAsync(token),
                await q.CountAsync(token));
        }
    }
}