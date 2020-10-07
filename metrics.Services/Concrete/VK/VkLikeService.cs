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
    public class VkLikeService : BaseHttpClient<VkLikeService>, IVkLikeService
    {
        public VkLikeService(
            IVkClient client,
            IJsonSerializer jsonSerializer,
            ILogger<VkLikeService> logger
        ) : base(client, jsonSerializer, logger)
        {
        }

        public Task<SimpleVkResponse<VkResponseLikeModel>> Like(VkRepostViewModel model)
        {
            var @params = new NameValueCollection
            {
                {"owner_id", $"{model.OwnerId}"},
                {"item_id", $"{model.Id}"},
                {"type", "post"}
            };
            return GetAsync<SimpleVkResponse<VkResponseLikeModel>>(VkApiUrls.Like, @params);
        }
    }
}