using metrics.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace metrics.Services.Abstract
{
    public interface IVkClient : IBaseHttpClient
    {
        VkResponse<List<VkMessage>> GetReposts(string id, int skip, int take, string search = null);
        void Repost(List<VkRepostViewModel> reposts, int timeout = 0);
        SimpleVkResponse<List<VkUserResponse>> GetUserInfo(string id);
    }
}
