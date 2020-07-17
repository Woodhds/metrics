using System;
using System.Linq;
using System.Threading.Tasks;
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

        [Test]
        public async Task TestCreateBatch()
        {
            using var scope = await Initializer.TransactionScopeFactory.CreateBatchAsync();

            await scope.CreateCollectionAsync(Enumerable.Range(0, 100).Select(_ => new VkRepost()));
            await scope.CommitAsync();

            Assert.NotZero(await scope.Query<VkRepost>().CountAsync());
        }
    }
}