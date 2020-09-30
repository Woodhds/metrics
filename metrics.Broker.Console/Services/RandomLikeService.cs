using System;
using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Authentication;
using metrics.Authentication.Services.Abstract;
using metrics.BackgroundJobs.Abstractions;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace metrics.Broker.Console.Services
{
    public interface IRandomLikeService
    {
        Task ExecuteRandomLike(int userId);
    }

    public class RandomLikeService : IRandomLikeService
    {
        private readonly IVkService _vkClient;
        private readonly ISecurityUserManager _securityUserManager;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IBackgroundJobService _backgroundJobService;

        public RandomLikeService(
            IVkService vkClient,
            ISecurityUserManager securityUserManager,
            ITransactionScopeFactory transactionScopeFactory,
            IBackgroundJobService schedulerJobService
        )
        {
            _vkClient = vkClient;
            _securityUserManager = securityUserManager;
            _transactionScopeFactory = transactionScopeFactory;
            _backgroundJobService = schedulerJobService;
        }

        public async Task ExecuteRandomLike(int userId)
        {
            var scope = _transactionScopeFactory.CreateQuery();

            var favouriteGroup = await scope
                .RawSql<FavouriteGroup>(
                    $"select * from public.\"FavouriteGroup\" where \"{nameof(FavouriteGroup.UserId)}\" = {userId} order by random() limit 1")
                .Select(f => f.GroupId)
                .FirstOrDefaultAsync();

            if (favouriteGroup != default)
            {
                using var user = _securityUserManager.SetUser(new SecurityUser {Id = userId});
                var rnd = new Random();
                var post = (await _vkClient.WallSearch(favouriteGroup, rnd.Next(1, 30), 1))?.Response?.Items
                    ?.FirstOrDefault();

                if (post != default && !(post.Likes?.User_Likes ?? true))
                {
                    await Task.Delay(300);
                    await _vkClient.Like(new VkRepostViewModel {Id = post.Id, OwnerId = post.OwnerId});
                }

                _backgroundJobService.Schedule<IRandomLikeService>(x => x.ExecuteRandomLike(userId),
                    TimeSpan.FromMilliseconds(rnd.Next(60 * 1000 * 3, 60 * 1000 * 60 * 5)));
            }
        }
    }
}