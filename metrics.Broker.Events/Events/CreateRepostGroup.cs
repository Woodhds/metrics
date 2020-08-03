using System.Collections.Generic;
using Base.Contracts;

namespace metrics.Broker.Events.Events
{
    public interface ICreateRepostGroup
    {
        int UserId { get; }

        IEnumerable<VkRepostViewModel> Reposts { get; }
    }
    
    public class CreateRepostGroup : ICreateRepostGroup
    {
        public int UserId { get; set; }

        public IEnumerable<VkRepostViewModel> Reposts { get; set; }
    }
}