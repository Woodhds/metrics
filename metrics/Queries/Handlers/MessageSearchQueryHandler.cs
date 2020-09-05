using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.EventSourcing.Abstractions.Query;
using metrics.Services.Abstractions;

namespace metrics.Queries.Handlers
{
    public class MessageSearchQueryHandler : IQueryHandler<MessageSearchQuery, DataSourceResponseModel>
    {
        private readonly IVkMessageService _vkMessageService;

        public MessageSearchQueryHandler(IVkMessageService vkMessageService)
        {
            _vkMessageService = vkMessageService;
        }

        public Task<DataSourceResponseModel> ExecuteAsync(MessageSearchQuery query, CancellationToken token = default)
        {
            var (page, pageSize, user, search) = query;
            
            return _vkMessageService.GetMessages(page, pageSize, search, user);
        }
    }
}