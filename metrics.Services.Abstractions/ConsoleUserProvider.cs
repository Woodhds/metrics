using metrics.Authentication;
using metrics.Authentication.Infrastructure;

namespace metrics.Services.Abstractions
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