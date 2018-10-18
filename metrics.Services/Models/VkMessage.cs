using System;
using System.Collections.Generic;
using System.Text;

namespace metrics.Services.Models
{
    public class VkMessage
    {
        public uint id { get; set; }
        public int owner_id { get; set; }
        public int from_id { get; set; }
        public uint date { get; set; }
        public string Text { get; set; }
        public PostType Post_type { get; set; }
        public List<VkMessage> Copy_History { get; set; }
    }

    public enum PostType
    {
        Post = 0
    }
}
