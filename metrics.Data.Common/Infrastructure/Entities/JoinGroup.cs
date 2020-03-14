using metrics.Data.Sql;

namespace metrics.Data.Common.Infrastructure.Entities
{
    public class JoinGroup : BaseEntity
    {
        public int GroupId { get; set; }
        public int Token { get; set; }
    }
}