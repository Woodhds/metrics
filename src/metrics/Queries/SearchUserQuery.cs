using System.Collections.Generic;
using Base.Contracts;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.EventSourcing.Abstractions.Query;

namespace metrics.Queries
{
    public record SearchUserQuery(string? Search) : IQuery<IEnumerable<VkUserModel>>;
}