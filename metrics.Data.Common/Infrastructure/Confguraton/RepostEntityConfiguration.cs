using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Common.Infrastructure.Confguraton
{
    public class RepostEntityConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder builder)
        {
            builder.Entity<JoinGroup>().HasOne(z => z.UserToken);
            builder.Entity<Repost>().HasOne(z => z.UserToken);
            builder.Entity<UserToken>().HasKey(g => g.UserId);
        }
    }
}