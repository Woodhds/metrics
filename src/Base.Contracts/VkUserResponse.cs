using System.Text.Json.Serialization;

namespace Base.Contracts
{
    public record VkUserResponse
    {
        public int Id { get; set; }
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
        [JsonPropertyName("photo_50")]
        public string Photo50 { get; set; }
    }
}