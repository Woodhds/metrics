using System.Collections.Generic;
using Base.Contracts;

namespace metrics.Broker.Events.Events
{
    public class CreateRepostGroup
    {
        public int UserId { get; set; }

        public IEnumerable<VkRepostViewModel> Reposts { get; set; }
    }
}