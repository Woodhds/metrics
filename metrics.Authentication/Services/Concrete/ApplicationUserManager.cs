using System;
using metrics.Authentication.Infrastructure;
using metrics.Authentication.Services.Abstract;

namespace metrics.Authentication.Services.Concrete
{
    public class ApplicationUserManager : ISecurityUserManager
    {
        private readonly IUserStore _userStore;

        public ApplicationUserManager(IUserStore userStore)
        {
            _userStore = userStore;
        }

        public IDisposable SetUser(IUser user)
        {
            if (user == null)
                throw new InvalidOperationException();

            return _userStore.SetValue(user);
        }
    }
}