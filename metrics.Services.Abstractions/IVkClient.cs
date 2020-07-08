using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstractions
{
    public interface IVkClient : IBaseHttpClient
    {
        Task<VkResponse<List<VkMessage>>> GetReposts(string id, int skip, int take, string search = null);
        Task Repost(List<VkRepostViewModel> reposts, int timeout = 0, int? userId = null);

        Task Repost(int ownerId, int id, int timeout = 0, int? userId = null) =>
            Repost(new List<VkRepostViewModel>() {new VkRepostViewModel(ownerId, id)}, timeout, userId);

        Task<SimpleVkResponse<List<VkUserResponse>>> GetUserInfo(string id, int? currentUser = null);

        Task<VkResponse<List<VkMessage>>> GetById(IEnumerable<VkRepostViewModel> vkRepostViewModels,
            int? userId = null);

        Task<VkResponse<List<VkGroup>>> GetGroups(int count, int offset);
        Task LeaveGroup(int groupId);
        Task<SimpleVkResponse<VkResponseLikeModel>> Like(VkRepostViewModel model);
        Task<VkResponse<IEnumerable<VkUserResponse>>> SearchUserAsync(string search, int? userId);
    }
}