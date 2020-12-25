using System.Text.Json;

namespace metrics.Serialization.Abstractions
{
    public interface IJsonSerializerOptionsProvider
    {
        JsonSerializerOptions Apply(JsonSerializerOptions? options);
    }
}