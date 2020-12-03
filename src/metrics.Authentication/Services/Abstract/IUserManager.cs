using System;
using metrics.Authentication.Infrastructure;

namespace metrics.Authentication.Services.Abstract
{
    public interface ISecurityUserManager
    {
        IDisposable SetUser(IUser user);
    }
}