using System.Text.Json.Serialization;
using metrics.Serialization;

namespace Base.Contracts
{
    public record VkLike
    {
        public uint Count { get; set; }
        [JsonConverter(typeof(JsonBooleanConverter))]
        [JsonPropertyName("user_likes")]
        public bool UserLikes { get; set; }
    }
}