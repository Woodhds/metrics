using System;
using System.ComponentModel.DataAnnotations;

namespace metrics.Data.Common.Infrastructure.Entities
{
    public class VkRepostUserOffset
    {
        [Key]
        public int UserId { get; set; }
        public DateTime LastPost { get; set; }
    }
}