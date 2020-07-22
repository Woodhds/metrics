using System.Text.Json.Serialization;
using metrics.Serialization;

namespace Base.Contracts
{
    public class VkRepostMessage
    {
        [JsonConverter(typeof(JsonBooleanConverter))]
        public bool Success { get; set; }
        public uint Post_Id { get; set; }
        public uint Reposts_Count { get; set; }
        public uint Likes_Count { get; set; }
    }
}