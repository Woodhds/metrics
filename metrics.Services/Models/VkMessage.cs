using System;
using System.Collections.Generic;
using System.Text;

namespace metrics.Services.Models
{
    public class VkMessage
    {
        public uint id { get; set; }
        public uint owner_id { get; set; }
        public uint from_id { get; set; }
        public uint date { get; set; }
    }
}
