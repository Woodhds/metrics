using metrics.Authentication;
using metrics.Authentication.Infrastructure;

namespace metrics.Competitions.Hosted.Services
{
    public class ConsoleUserProvider : IAuthenticatedUserProvider
    {
        public IUser GetUser()
        {
            return new SecurityUser
            {
                Id = 0
            };
        }
    }
}