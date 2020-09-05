using System.Collections.Generic;
using Base.Contracts;
using metrics.EventSourcing.Abstractions.Query;

namespace metrics.Queries
{
    public class SearchUserQuery : IQuery<IEnumerable<VkUserModel>>
    {
        public string Search { get; set; }
    }
}