using System;
using metrics.Data.Common.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace metrics.Data.Sql.Tests
{
    [TestFixture]
    public class RepositoryTests
    {
        [Test]
        public void TestCreate()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                using var scope = await Initializer.TransactionScopeFactory.CreateAsync();
                var repost = new VkRepost();
                await scope.GetRepository<VkRepost>().CreateAsync(repost);
                Assert.True(repost.Id != 0);
            });
        }

        [Test]
        public void TestUpdate()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                using var scope = await Initializer.TransactionScopeFactory.CreateAsync();
                var repost = new VkRepost();
                await scope.GetRepository<VkRepost>().CreateAsync(repost);
                repost.Status = VkRepostStatus.Complete;
                await scope.GetRepository<VkRepost>().UpdateAsync(repost);
                Assert.IsTrue(await scope.Query<VkRepost>().AnyAsync(e => e.Status == VkRepostStatus.Complete));
            });
        }
    }
}