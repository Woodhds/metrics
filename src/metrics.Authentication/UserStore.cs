using System;
using metrics.Authentication.Infrastructure;

namespace metrics.Authentication
{
    public class UserStore : IUserStore
    {
        private UserEntry _entry;

        public IDisposable SetValue(IUser user)
        {
            _entry = new UserEntry(GetValue(), user);

            return _entry;
        }

        public IUser Value => GetValue();

        private IUser GetValue()
        {
            return _entry != null ? _entry.Disposed ? _entry.Prev : _entry.Next : null;
        }


        private class UserEntry : IDisposable
        {
            public bool Disposed { get; private set; }
            public readonly IUser Prev;
            public readonly IUser Next;

            public UserEntry(IUser prev, IUser next)
            {
                Prev = prev;
                Next = next;
            }

            public void Dispose()
            {
                Disposed = true;
            }
        }
    }
}