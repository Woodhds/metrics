using System.Collections.Generic;

namespace Base.Contracts.Options
{
    public record VkontakteOptions
    {
        public string ApiVersion { get; init; } = "";

        public string AppId { get; init; } = "";
        public string AppSecret { get; init; } = "";
        public string AppScope { get; init; } = "";
        
        public HashSet<string> Fields { get; init; } = new();
    }
}