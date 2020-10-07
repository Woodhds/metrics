using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstractions.VK
{
    public interface IVkGroupService
    {
        Task<VkResponse<List<VkGroup>>> GetGroups(int count, int offset);
        Task LeaveGroup(int groupId);
        Task JoinGroup(int groupId);
    }
}