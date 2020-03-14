using metrics.Data.Sql;

namespace metrics.Data.Common.Infrastructure.Entities
{
    public class Repost : BaseEntity
    {
        public int PostId { get; set; }
        public int OwnerId { get; set; }
        public int Token { get; set; }
    }
}