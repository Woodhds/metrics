using System.Text.Json.Serialization;
using metrics.Serialization;

namespace Base.Contracts
{
    public class VkGroup : Owner
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(JsonBooleanConverter))]
        [JsonPropertyName("is_member")]
        public bool IsMember { get; set; }
    }
}
