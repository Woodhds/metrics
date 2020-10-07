using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
    public class VkWallService : BaseHttpClient<VkWallService>, IVkWallService
    {
        
        public VkWallService(IVkClient client, IJsonSerializer jsonSerializer, ILogger<VkWallService> logger) : base(client, jsonSerializer, logger)
        {
        }
        
        public Task<VkResponse<List<VkMessage>>> WallSearch(string id, int skip, int take, string? search = null)
        {
            var @params = new NameValueCollection
            {
                {"count", take.ToString()},
                {"offset", ((skip - 1) * take).ToString()},
                {"filter", "owner"},
                {"owner_id", id}
            };
            var method = VkApiUrls.Wall;
            if (!string.IsNullOrEmpty(search))
            {
                method = VkApiUrls.WallSearch;
                @params.Add("query", search);
            }

            return base.GetAsync<VkResponse<List<VkMessage>>>(method, @params);
        }

        public Task<VkResponse<List<VkMessage>>> GetById(IEnumerable<VkRepostViewModel>? vkRepostViewModels)
        {
            if (vkRepostViewModels == null)
            {
                throw new ArgumentNullException(nameof(vkRepostViewModels));
            }

            var @params = new NameValueCollection
            {
                {"posts", string.Join(",", vkRepostViewModels.Select(c => $"{c.OwnerId}_{c.Id}"))},
                {"extended", 1.ToString()},
                {"fields", "is_member"}
            };
            return base.GetAsync<VkResponse<List<VkMessage>>>(VkApiUrls.WallGetById, @params);
        }

        public Task<SimpleVkResponse<VkRepostMessage>> Repost(VkRepostViewModel model)
        {
            var @params = new NameValueCollection
            {
                {"object", $"wall{model.OwnerId}_{model.Id}"}
            };
            
            return base.PostAsync<SimpleVkResponse<VkRepostMessage>>(VkApiUrls.Repost, null, @params);
        }
    }
}