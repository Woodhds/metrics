using System;

namespace metrics.Authentication.Infrastructure
{
    public interface IUserStore
    {
        IDisposable SetValue(IUser user);
        IUser Value { get; }
    }
}