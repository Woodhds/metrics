using Base.Contracts;
using metrics.EventSourcing.Abstractions.Query;

namespace metrics.Queries
{
    public class MessageCategoryTypesQuery : IQuery<DataSourceResponseModel>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public void Deconstruct(out int page, out int pageSize)
        {
            page = Page;
            pageSize = PageSize;
        }
    }
}