using System.Text.Json;
using System.Text.Json.Serialization;
using metrics.Serialization.Abstractions;

namespace metrics.Serialization
{
    public class JsonSerializerOptionsProvider : IJsonSerializerOptionsProvider
    {
        public JsonSerializerOptions Apply(JsonSerializerOptions options)
        {
            options ??= new JsonSerializerOptions();

            options.Converters.Add(new JsonStringEnumConverter());
            options.PropertyNameCaseInsensitive = true;
            options.PropertyNamingPolicy = null;

            return options;
        }
    }
}