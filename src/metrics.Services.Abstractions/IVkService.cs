using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstractions
{
    public interface IVkService
    {
        Task<VkResponse<List<VkMessage>>> GetReposts(string id, int skip, int take, string? search = null);
        Task Repost(List<VkRepostViewModel> reposts);

        Task Repost(int ownerId, int id) =>
            Repost(new List<VkRepostViewModel> {new VkRepostViewModel(ownerId, id)});
    }
}