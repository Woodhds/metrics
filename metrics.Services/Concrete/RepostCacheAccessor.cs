using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace metrics.Services.Concrete
{
    public interface IRepostCacheAccessor
    {
        Task<IEnumerable<(int userId, VkRepostViewModel repost, DateTime last)>> GetAsync(
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

        public async Task<IEnumerable<(int userId, VkRepostViewModel repost, DateTime last)>> GetAsync(
            CancellationToken cancellationToken = default)
        {
            using var scope = await _transactionScopeFactory.CreateAsync(cancellationToken: cancellationToken);

            var list = await (from r in scope.GetRepository<VkRepost>()
                    .Read()
                    .Where(f => f.Status == VkRepostStatus.New)
                from t in scope.GetRepository<VkRepostUserOffset>().Read().Where(q => q.UserId == r.UserId)
                    .DefaultIfEmpty()
                select new
                {
                    r.UserId,
                    r.MessageId,
                    r.OwnerId,
                    LastPost = t,
                    r.Id
                }).ToListAsync(cancellationToken);

            foreach (var entity in list)
            {
                await scope.GetRepository<VkRepost>().UpdateAsync(new VkRepost
                {
                    Id = entity.Id,
                    Status = VkRepostStatus.Pending,
                    DateStatus = DateTime.Now,
                    MessageId = entity.MessageId,
                    OwnerId = entity.OwnerId,
                    UserId = entity.UserId
                }, cancellationToken);
            }

            await scope.CommitAsync(cancellationToken);

            return list.Select(q =>
                (q.UserId, new VkRepostViewModel(q.OwnerId, q.MessageId), q.LastPost?.LastPost ?? DateTime.Now));
        }

        public async Task SetAsync(int userId, IEnumerable<VkRepostViewModel> models)
        {
            using var scope = await _transactionScopeFactory.CreateAsync();

            var obj = models.Select(f => new
            {
                f.Id,
                f.Owner_Id,
                Key = f.Id + "_" + f.Owner_Id
            }).ToArray();

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
                .Count(f => f.UserId == userId &&
                            (f.Status == VkRepostStatus.New || f.Status == VkRepostStatus.Pending));
        }
    }
}