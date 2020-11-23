using Base.Contracts;
using metrics.EventSourcing.Abstractions.Query;

namespace metrics.Queries
{
    public record MessageSearchQuery(int Page, int PageSize, string? Search) : IQuery<DataSourceResponseModel>;
}