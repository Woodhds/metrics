using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstractions
{
    public interface IVkUserService
    {
        Task<VkUserModel> CreateAsync(string userId, int? currentUser = null);
        Task<IEnumerable<VkUserModel>> Get(string searchStr);
        Task<VkResponse<IEnumerable<VkUserResponse>>> SearchAsync(string search, int? userId);
    }
}