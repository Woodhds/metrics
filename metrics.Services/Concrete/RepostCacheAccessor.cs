using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;

namespace metrics.Services.Concrete
{
    public interface IRepostCacheAccessor
    {
        Task<IEnumerable<(int userId, VkRepostViewModel repost)>> GetAsync(
            CancellationToken cancellationToken = default);

        Task SetAsync(int userId, IEnumerable<VkRepostViewModel> models);

        ValueTask<int> GetCountAsync(int userId);
    }

    public class RepostCacheAccessor : IRepostCacheAccessor
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public RepostCacheAccessor(ITransactionScopeFactory transactionScopeFactory)
        {
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task<IEnumerable<(int userId, VkRepostViewModel repost)>> GetAsync(
            CancellationToken cancellationToken = default)
        {
            using var scope = await _transactionScopeFactory.CreateAsync(cancellationToken: cancellationToken);

            var list = scope.GetRepository<VkRepost>().Read().Where(f => f.Status == VkRepostStatus.Pending || f.Status == VkRepostStatus.New)
                .AsEnumerable()
                .GroupBy(q => new {q.UserId})
                .Select(q => new
                {
                    q.Key.UserId,
                    Repost = q.FirstOrDefault(),
                })
                .Where(f => f.Repost != null)
                .ToList();

            foreach (var entity in list)
            {
                entity.Repost.Status = VkRepostStatus.Pending;
                entity.Repost.DateStatus = DateTime.Now;
                await scope.GetRepository<VkRepost>().UpdateAsync(entity.Repost);
            }

            await scope.CommitAsync(cancellationToken);

            return list.Select(q => (q.UserId,
                q.Repost != null
                    ? new VkRepostViewModel {Id = q.Repost.MessageId, Owner_Id = q.Repost.OwnerId}
                    : null));
        }

        public async Task SetAsync(int userId, IEnumerable<VkRepostViewModel> models)
        {
            using var scope = await _transactionScopeFactory.CreateAsync();

            var obj = models.Select(f => new
            {
                f.Id,
                f.Owner_Id,
                Key = f.Id + "_" + f.Owner_Id
            });
            var keys = obj.Select(f => f.Key);

            var alreadyCreate = scope.GetRepository<VkRepost>().Read()
                .Where(f => f.UserId == userId &&
                            (f.Status == VkRepostStatus.New || f.Status == VkRepostStatus.Pending))
                .Select(f => new
                {
                    f.MessageId,
                    f.OwnerId,
                    key = f.MessageId.ToString() + "_" + f.OwnerId.ToString()
                })
                .Where(f => keys.Contains(f.key))
                .ToList();

            var toCreate = obj.Select(f => f.Key).Except(alreadyCreate.Select(f => f.key));
            foreach (var create in obj.Where(f => toCreate.Contains(f.Key)))
            {
                await scope.GetRepository<VkRepost>().CreateAsync(new VkRepost
                {
                    Status = VkRepostStatus.New,
                    MessageId = create.Id,
                    DateStatus = DateTime.Now,
                    OwnerId = create.Owner_Id,
                    UserId = userId
                });
            }

            await scope.CommitAsync();
        }

        public async ValueTask<int> GetCountAsync(int userId)
        {
            return (await _transactionScopeFactory.CreateAsync()).GetRepository<VkRepost>().Read()
                .Count(f => f.UserId == userId && (f.Status == VkRepostStatus.New || f.Status == VkRepostStatus.Pending));
        }
    }
}