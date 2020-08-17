using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using Elastic.Client;
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

        public async Task<VkUserModel> CreateAsync(string userId)
        {
            var userInfo = await _vkClient.GetUserInfo(userId);

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
                await _elasticClientProvider.Create().IndexDocumentAsync(user);
            }

            return user;
        }

        public async Task<IEnumerable<VkUserModel>> GetAsync(string searchStr)
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
            

            return result.IsValid 
                ? result.Documents 
                : Enumerable.Empty<VkUserModel>();
        }

        public Task<VkResponse<IEnumerable<VkUserResponse>>> SearchAsync(string search)
        {
            return _vkClient.SearchUserAsync(search);
        }
    }
}