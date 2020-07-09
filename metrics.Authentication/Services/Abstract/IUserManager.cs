using System;

namespace metrics.Authentication.Services.Abstract
{
    public interface ISecurityUserManager
    {
        IDisposable SetUser(IUser user);
    }
}