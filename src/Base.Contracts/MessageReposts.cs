using System.Text.Json.Serialization;
using metrics.Serialization;

namespace Base.Contracts
{
    public class MessageReposts
    {
        [JsonConverter(typeof(JsonBooleanConverter))]
        [JsonPropertyName("user_reposted")]
        public bool UserReposted { get; set; }
        public uint Count { get; set; }
    }
}
