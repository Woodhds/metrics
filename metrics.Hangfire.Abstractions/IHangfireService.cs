using System;

namespace metrics.Hangfire.Abstractions
{
    public interface IHangfireService
    {
        void RegisterRepost(int id, int ownerId, TimeSpan delay);
        void RegisterJoin(int id, TimeSpan delay);
    }
}