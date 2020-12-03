using System.Text.Json.Serialization;
using metrics.Serialization;

namespace Base.Contracts
{
    public class MessageReposts
    {
        [JsonConverter(typeof(JsonBooleanConverter))]
        public bool User_reposted { get; set; }
        public uint Count { get; set; }
    }
}
