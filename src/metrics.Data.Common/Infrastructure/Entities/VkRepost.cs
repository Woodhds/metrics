using System;
using metrics.Data.Abstractions;

namespace metrics.Data.Common.Infrastructure.Entities
{
    public class VkRepost : BaseEntity
    {
        public int UserId { get; set; }
        public int OwnerId { get; set; }
        public int MessageId { get; set; }
        public VkRepostStatus Status { get; set; }
        public DateTime DateStatus { get; set; }
    }
}