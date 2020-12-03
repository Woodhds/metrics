using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Data.Abstractions;
using metrics.Services.Abstractions;
using metrics.Services.Abstractions.VK;
using Microsoft.EntityFrameworkCore;

namespace metrics.Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IVkUserService _vkClient;

        public UserService(IVkUserService vkClient, ITransactionScopeFactory transactionScopeFactory)
        {
            _vkClient = vkClient;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task<VkUserModel> CreateAsync(string userId, CancellationToken token = default)
        {
            var userInfo = await _vkClient.GetUserInfo(userId);

            if (userInfo.Response == null)
                throw new ArgumentNullException(nameof(userId));

            var user = new VkUserModel
            {
                Id = userInfo.Response.First().Id,
                FullName = userInfo.Response.First()?.FirstName + " " + userInfo.Response.First()?.LastName,
                Avatar = userInfo.Response.First()?.Photo50
            };
            if (user.Id > 0)
            {
                using var scope = await _transactionScopeFactory.CreateAsync(token);
                await scope.GetRepository<VkUserModel>().CreateAsync(user, token);
                await scope.CommitAsync(token);
            }

            return user;
        }

        public async Task<IEnumerable<VkUserModel>> GetAsync(string? searchStr, CancellationToken ct = default)
        {
            using var scope = await _transactionScopeFactory.CreateAsync(ct);
            var query = scope.Query<VkUserModel>();

            if (!string.IsNullOrEmpty(searchStr))
            {
                query = query.Where(f => EF.Functions.ToTsVector("russian", f.FullName).Matches(searchStr));
            }

            return await query.ToListAsync(ct);
        }

        public Task<VkResponse<IEnumerable<VkUserResponse>>> SearchAsync(string search, CancellationToken ct = default)
        {
            return _vkClient.SearchUserAsync(search);
        }
    }
}