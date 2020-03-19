using metrics.Data.Sql;

namespace metrics.Data.Common.Infrastructure.Entities
{
    public class JoinGroup : BaseEntity
    {
        public int GroupId { get; set; }
        public int UserTokenId { get; set; }
        public UserToken UserToken { get; set; }
    }
}