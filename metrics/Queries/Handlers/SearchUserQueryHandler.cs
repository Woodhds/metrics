using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.EventSourcing.Abstractions.Query;
using metrics.Services.Abstractions;

namespace metrics.Queries.Handlers
{
    public class SearchUserQueryHandler : IQueryHandler<SearchUserQuery, IEnumerable<VkUserModel>>
    {
        private readonly IVkUserService _vkUserService;

        public SearchUserQueryHandler(IVkUserService vkUserService)
        {
            _vkUserService = vkUserService;
        }

        public async Task<IEnumerable<VkUserModel>> ExecuteAsync(SearchUserQuery query,
            CancellationToken token = default)
        {
            return (await _vkUserService.SearchAsync(query.Search))?.Response?.Items?.Select(q =>
                new VkUserModel
                {
                    Avatar = q.Photo_50,
                    Id = q.Id,
                    FullName = q.First_name + " " + q.Last_Name
                });
        }
    }
}