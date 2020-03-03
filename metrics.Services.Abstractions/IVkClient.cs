using System.Collections.Generic;
using Base.Contracts;

namespace metrics.Services.Abstractions
{
    public interface IVkClient : IBaseHttpClient
    {
        VkResponse<List<VkMessage>> GetReposts(string id, int skip, int take, string search = null);
        void Repost(List<VkRepostViewModel> reposts, int timeout = 0);
        SimpleVkResponse<List<VkUserResponse>> GetUserInfo(string id);
        VkResponse<List<VkMessage>> GetById(IEnumerable<VkRepostViewModel> vkRepostViewModels);
        VkResponse<List<VkGroup>> GetGroups(int count, int offset);
        void LeaveGroup(int groupId);
        SimpleVkResponse<VkResponseLikeModel> Like(VkRepostViewModel model);
    }
}
