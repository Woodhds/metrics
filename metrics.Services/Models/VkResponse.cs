using System;
using System.Collections.Generic;
using System.Text;

namespace metrics.Services.Models
{
    public class VkResponse<T>
    {
        public VkResponseItems<T> Response { get; set; }

        public class VkResponseItems<T>
        {
            public int Count { get; set; }
            public T Items { get; set; }
            public List<VkGroup> Groups { get; set; }
        }
    }
}
