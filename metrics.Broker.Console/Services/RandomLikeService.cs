using System.Threading.Tasks;
using metrics.Authentication;
using metrics.Authentication.Services.Abstract;
using metrics.Services.Abstractions;

namespace metrics.Broker.Console.Services
{
    public interface IRandomLikeService
    {
        Task ExecuteRandomLike();
    }

    public class RandomLikeService : IRandomLikeService
    {
        private readonly IVkClient _vkClient;
        private readonly ISecurityUserManager _securityUserManager;

        public RandomLikeService(IVkClient vkClient, ISecurityUserManager securityUserManager)
        {
            _vkClient = vkClient;
            _securityUserManager = securityUserManager;
        }

        public async Task ExecuteRandomLike()
        {
            using var user = _securityUserManager.SetUser(new SecurityUser {Id = 68868143});
            var groups = (await _vkClient.GetGroups(100, 0))?.Response.Groups;
            
        }
    }
}