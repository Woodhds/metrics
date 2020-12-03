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
    public class VkGroupService : BaseHttpClient<VkGroupService>, IVkGroupService
    {
        public VkGroupService(
            IVkClient client,
            IJsonSerializer jsonSerializer,
            ILogger<VkGroupService> logger
        ) : base(client, jsonSerializer, logger)
        {
        }
        
        public async Task JoinGroup(int groupId)
        {
            var @params = new NameValueCollection
            {
                {"group_id", groupId.ToString()}
            };

            await base.GetAsync<SimpleVkResponse<int>>(VkApiUrls.GroupJoin, @params);
        }

        public Task<VkResponse<List<VkGroup>>> GetGroups(int count, int offset)
        {
            var @params = new NameValueCollection
            {
                {"count", $"{count}"},
                {"fields", "name, description"},
                {"extended", "1"},
                {"offset", $"{offset}"}
            };
            return base.GetAsync<VkResponse<List<VkGroup>>>(VkApiUrls.Groups, @params);
        }

        public Task LeaveGroup(int groupId)
        {
            var @params = new NameValueCollection
            {
                {"group_id", $"{groupId}"}
            };
            return base.GetAsync<SimpleVkResponse<string>>(VkApiUrls.LeaveGroup, @params);
        }
    }
}