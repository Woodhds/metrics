using System.Collections.Generic;

namespace metrics.Services.Extensions
{
    public class VkRequestState
    {
        public int UserId { get; set; }
        public string Method { get; set; }
        public string ApiVersion { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}