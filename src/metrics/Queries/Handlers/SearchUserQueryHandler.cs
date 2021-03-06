﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.EventSourcing.Abstractions.Query;
using metrics.Services.Abstractions;

namespace metrics.Queries.Handlers
{
    public class SearchUserQueryHandler : IQueryHandler<SearchUserQuery, IEnumerable<VkUserModel>>
    {
        private readonly IUserService _vkUserService;

        public SearchUserQueryHandler(IUserService vkUserService)
        {
            _vkUserService = vkUserService;
        }

        public async Task<IEnumerable<VkUserModel>> ExecuteAsync(SearchUserQuery query,
            CancellationToken token = default)
        {
            return (await _vkUserService.SearchAsync(query.Search))?.Response?.Items?.Select(q =>
                new VkUserModel
                {
                    Avatar = q.Photo50,
                    Id = q.Id,
                    FullName = q.FirstName + " " + q.LastName
                });
        }
    }
}