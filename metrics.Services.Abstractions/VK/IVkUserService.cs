using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstractions.VK
{
    public interface IVkUserService
    {
        Task<VkResponse<IEnumerable<VkUserResponse>>> SearchUserAsync(string search);
        Task<SimpleVkResponse<List<VkUserResponse>>> GetUserInfo(string id);
    }
}