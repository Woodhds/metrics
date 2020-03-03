using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstractions
{
    public interface IVkUserService
    {
        Task<VkUserModel> CreateAsync(string userId, CancellationToken token = default);
        Task<IEnumerable<VkUserModel>> SearchAsync(string searchStr);
    }
}