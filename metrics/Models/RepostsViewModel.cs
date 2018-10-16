using metrics.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace metrics.Models
{
    public class RepostsViewModel
    {
        public string Filter { get; set; }
        public List<VkMessage> Messages { get; set; } = new List<VkMessage>();
    }

    public class FilterPostsViewModel
    {
        public string User { get; set; }
    }
}
