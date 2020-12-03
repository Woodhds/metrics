using metrics.Authentication.Infrastructure;

namespace metrics.Authentication
{
    public class AuthenticatedUserProvider : IAuthenticatedUserProvider
    {
        private readonly IUserStore _userStore;

        public AuthenticatedUserProvider(IUserStore userStore)
        {
            _userStore = userStore;
        }

        public IUser GetUser()
        {
            return _userStore.Value;
        }
    }
}