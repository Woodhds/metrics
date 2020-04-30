using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Common.Infrastructure.Confguraton
{
    public class RepostEntityConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder builder)
        {
            builder.Entity<UserToken>().HasKey(g => g.UserId);

            builder.Entity<VkRepost>().HasKey(q => q.Id);

            builder.Entity<MessageCategory>().HasKey(q => q.Id);

            var messageBuilder = builder.Entity<MessageVk>();
            messageBuilder.HasOne(z => z.MessageCategory);
            messageBuilder.HasKey(q => new {q.MessageId, q.OwnerId});
        }
    }
}