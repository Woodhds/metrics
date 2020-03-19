using System;
using System.Collections.Generic;
using Base.Contracts;

namespace metrics.Hangfire.Abstractions
{
    public interface IHangfireService
    {
        void RegisterRepost(IEnumerable<VkRepostViewModel> reposts, int timeout = 60);
    }
}