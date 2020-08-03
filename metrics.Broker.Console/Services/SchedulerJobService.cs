using System.Threading.Tasks;
using metrics.Authentication;
using metrics.Authentication.Services.Abstract;
using metrics.Services.Abstractions;

namespace metrics.Broker.Console.Services
{
    public interface ISchedulerJobService
    {
        Task Repost(int ownerId, int messageId, int userId);
    }

    public class SchedulerJobService : ISchedulerJobService
    {
        private readonly IVkClient _vkClient;
        private readonly ISecurityUserManager _userManager;

        public SchedulerJobService(IVkClient vkClient, ISecurityUserManager userManager)
        {
            _vkClient = vkClient;
            _userManager = userManager;
        }

        public async Task Repost(int ownerId, int messageId, int userId)
        {
            using (_userManager.SetUser(new SecurityUser {Id = userId}))
            {
                await _vkClient.Repost(ownerId, messageId, 1);
            }
        }
    }
}