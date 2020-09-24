using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstractions
{
    public interface IVkUserService
    {
        Task<VkUserModel> CreateAsync(string userId, CancellationToken ct = default);
        Task<IEnumerable<VkUserModel>> GetAsync(string? searchStr, CancellationToken ct = default);
        Task<VkResponse<IEnumerable<VkUserResponse>>> SearchAsync(string? search, CancellationToken ct = default);
    }
}