﻿using Base.Contracts;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Common.Infrastructure.Configuration
{
    public class RepostEntityConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder builder)
        {
            builder.Entity<VkRepost>().HasKey(q => q.Id);

            builder.Entity<MessageCategory>().HasKey(q => q.Id);

            var messageBuilder = builder.Entity<MessageVk>();
            messageBuilder.HasOne(z => z.MessageCategory);
            messageBuilder.HasKey(q => new {q.MessageId, q.OwnerId});

            builder.Entity<FavouriteGroup>().HasKey(f => new {f.GroupId, f.UserId});
            var vkUserBuilder = builder.Entity<VkUserModel>();
            vkUserBuilder.HasKey(x => x.Id);
            vkUserBuilder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
        }
    }
}