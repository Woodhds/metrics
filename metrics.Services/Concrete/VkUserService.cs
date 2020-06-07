using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Abstractions;
using Base.Contracts;
using metrics.Services.Abstractions;

namespace metrics.Services.Concrete
{
    public class VkUserService : IVkUserService
    {
        private readonly IElasticClientFactory _elasticClientProvider;
        private readonly IVkClient _vkClient;

        public VkUserService(IElasticClientFactory elasticClientProvider, IVkClient vkClient)
        {
            _elasticClientProvider = elasticClientProvider;
            _vkClient = vkClient;
        }

        public async Task<VkUserModel> CreateAsync(string userId, int? currentUser = null, CancellationToken token = default)
        {
            var userInfo = await _vkClient.GetUserInfo(userId, currentUser);

            if (userInfo.Response == null)
                throw new ArgumentNullException(nameof(userId));
            
            var user = new VkUserModel
            {
                Id = userInfo.Response.First().Id,
                FullName = userInfo.Response.First()?.First_name + " " + userInfo.Response.First()?.Last_Name,
                Avatar = userInfo.Response.First()?.Photo_50
            };
            if (user.Id > 0)
            {
                await _elasticClientProvider.Create().IndexDocumentAsync(user, token);
            }

            return user;
        }

        public async Task<IEnumerable<VkUserModel>> SearchAsync(string searchStr)
        {
            var result = await _elasticClientProvider.Create().SearchAsync<VkUserModel>(z =>
                z.Index<VkUserModel>()
                    .Query(t =>
                        t.QueryString(a =>
                            a.Query(searchStr)
                                .Fields(e => e.Field(g => g.FullName))
                                .Analyzer("russian")
                        )
                    )
            );
            

            if (result.IsValid)
            {
                return result.Documents;
            }

            return Enumerable.Empty<VkUserModel>();
        }
    }
}