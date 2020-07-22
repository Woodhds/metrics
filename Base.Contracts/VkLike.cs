using System.Text.Json.Serialization;
using metrics.Serialization;

namespace Base.Contracts
{
    public class VkLike
    {
        public int Count { get; set; }
        [JsonConverter(typeof(JsonBooleanConverter))]
        public bool User_Likes { get; set; }
    }
}