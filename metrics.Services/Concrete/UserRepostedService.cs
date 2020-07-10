using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;

namespace metrics.Services.Concrete
{
    public interface IUserRepostedService
    {
        Task SetAsync(int userId, IEnumerable<VkRepostViewModel> models);

        ValueTask<int> GetCountAsync(int userId);
    }

    public class UserRepostedService : IUserRepostedService
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public UserRepostedService(ITransactionScopeFactory transactionScopeFactory)
        {
            _transactionScopeFactory = transactionScopeFactory;
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