using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstractions
{
    public interface IVkClient : IBaseHttpClient
    {
        Task<VkResponse<List<VkMessage>>> GetReposts(string id, int skip, int take, string search = null);
        Task Repost(List<VkRepostViewModel> reposts, int timeout = 0);
        Task<SimpleVkResponse<List<VkUserResponse>>> GetUserInfo(string id);
        Task<VkResponse<List<VkMessage>>> GetById(IEnumerable<VkRepostViewModel> vkRepostViewModels);
        Task<VkResponse<List<VkGroup>>> GetGroups(int count, int offset);
        Task LeaveGroup(int groupId);
        Task<SimpleVkResponse<VkResponseLikeModel>> Like(VkRepostViewModel model);
    }
}
