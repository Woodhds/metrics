using System.Collections.Generic;
using Base.Contracts;

namespace metrics.Broker.Events.Events
{
    public class GroupJoinEvent
    {
        public int UserId { get; set; }
        
        public string Token { get; set; }
        
        public IEnumerable<VkGroup> Groups { get; set; }
    }
}