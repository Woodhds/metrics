using System.Collections.Generic;
using Base.Contracts;

namespace metrics.Broker.Events.Events
{
    public class RepostGroupCreatedEvent
    {
        public int UserId { get; set; }
        public int Timeout { get; set; } = 30;

        public IEnumerable<VkRepostViewModel> Reposts { get; set; }
    }
}