using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstractions.VK
{
    public interface IVkWallService
    {
        Task<VkResponse<List<VkMessage>>> WallSearch(string id, int skip, int take, string? search = null);
        Task<VkResponse<List<VkMessage>>> GetById(IEnumerable<VkRepostViewModel>? vkRepostViewModels);
        Task<SimpleVkResponse<VkRepostMessage>> Repost(VkRepostViewModel model);

    }
}