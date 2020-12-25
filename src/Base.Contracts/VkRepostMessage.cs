using System.Text.Json.Serialization;
using metrics.Serialization;

namespace Base.Contracts
{
    public class VkRepostMessage
    {
        [JsonConverter(typeof(JsonBooleanConverter))]
        public bool Success { get; set; }
        [JsonPropertyName("post_id")]
        public uint PostId { get; set; }
        [JsonPropertyName("reposts_count")]
        public uint RepostsCount { get; set; }
        [JsonPropertyName("likes_count")]
        public uint LikesCount { get; set; }
    }
}