using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Base.Contracts;
using Base.Contracts.Options;
using metrics.Serialization.Abstractions;
using metrics.Services.Abstractions;
using metrics.Services.Abstractions.VK;
using metrics.Services.Utils;
using Microsoft.Extensions.Logging;

namespace metrics.Services.Concrete.VK
{
    public class VkUserService : BaseHttpClient<VkUserService>, IVkUserService
    {
        public VkUserService(IVkClient client, IJsonSerializer jsonSerializer, ILogger<VkUserService> logger) : base(client, jsonSerializer, logger)
        {
        }

        public Task<VkResponse<IEnumerable<VkUserResponse>>> SearchUserAsync(string search)
        {
            var @params = new NameValueCollection
            {
                {"q", search},
                {"count", "100"},
                {"fields", "photo_50"}
            };

            return base.GetAsync<VkResponse<IEnumerable<VkUserResponse>>>(VkApiUrls.UserSearch, @params);
        }

        
        public Task<SimpleVkResponse<List<VkUserResponse>>> GetUserInfo(string id)
        {
            var @params = new NameValueCollection
            {
                {"user_ids", id},
                {"fields", "first_name,last_name,photo_50"}
            };

            return base.GetAsync<SimpleVkResponse<List<VkUserResponse>>>(VkApiUrls.UserInfo, @params);
        }
    }
}