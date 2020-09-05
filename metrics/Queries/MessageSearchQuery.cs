using Base.Contracts;
using metrics.EventSourcing.Abstractions.Query;

namespace metrics.Queries
{
    public class MessageSearchQuery : IQuery<DataSourceResponseModel>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string User { get; set; }
        public string Search { get; set; }

        public void Deconstruct(out int page, out int pageSize, out string user, out string search)
        {
            page = Page;
            pageSize = PageSize;
            user = User;
            search = Search;
        }
    }
}