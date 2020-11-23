using Base.Contracts;
using metrics.EventSourcing.Abstractions.Query;

namespace metrics.Queries
{
    public record MessageCategoryTypesQuery(int Page, int PageSize) : IQuery<DataSourceResponseModel>;
}