using System.Text.Json;
using metrics.Serialization.Abstractions;

namespace metrics.Serialization
{
    public class JsonSerializer : IJsonSerializer
    {
        public T Deserialize<T>(string input)
        {
            return string.IsNullOrEmpty(input) ? default 
                : System.Text.Json.JsonSerializer.Deserialize<T>(input, new JsonSerializerOptions());
        }

        public string Serialize<T>(T value)
        {
            return System.Text.Json.JsonSerializer.Serialize(value);
        }
    }
}