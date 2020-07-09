using System;

namespace metrics.Authentication
{
    public interface IUserStore
    {
        IDisposable SetValue(IUser user);
        IUser Value { get; }
    }
}