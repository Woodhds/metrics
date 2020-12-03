using System.Threading.Tasks;
using Hangfire;
using metrics.Authentication;
using metrics.Authentication.Services.Abstract;
using metrics.Services.Abstractions;

namespace metrics.Broker.Console.Services
{
    public interface ISchedulerJobService
    {
        [Queue("repost")]
        Task Repost(int ownerId, int messageId, int userId);
    }

    public class SchedulerJobService : ISchedulerJobService
    {
        private readonly IVkService _vkClient;
        private readonly ISecurityUserManager _userManager;

        public SchedulerJobService(IVkService vkClient, ISecurityUserManager userManager)
        {
            _vkClient = vkClient;
            _userManager = userManager;
        }

        public async Task Repost(int ownerId, int messageId, int userId)
        {
            using (_userManager.SetUser(new SecurityUser {Id = userId}))
            {
                await _vkClient.Repost(ownerId, messageId);
            }
        }
    }
}