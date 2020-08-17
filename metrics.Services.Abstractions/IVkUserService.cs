using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstractions
{
    public interface IVkUserService
    {
        Task<VkUserModel> CreateAsync(string userId);
        Task<IEnumerable<VkUserModel>> GetAsync(string searchStr);
        Task<VkResponse<IEnumerable<VkUserResponse>>> SearchAsync(string search);
    }
}