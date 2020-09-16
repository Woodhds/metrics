using System.Collections.Generic;

namespace Base.Contracts.Options
{
    public class VkontakteOptions
    {
        public string ApiVersion { get; set; } = "";

        public string AppId { get; set; } = "";
        public string AppSecret { get; set; } = "";
        public string AppScope { get; set; } = "";
        
        public HashSet<string> Fields { get; set; } = new HashSet<string>();
    }
}