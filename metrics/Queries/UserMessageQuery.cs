using Base.Contracts;
using metrics.EventSourcing.Abstractions.Query;

namespace metrics.Queries
{
    public record UserMessageQuery(int Page, int PageSize) : IQuery<DataSourceResponseModel>;
}