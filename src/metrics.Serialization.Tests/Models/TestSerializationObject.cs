using System;
using System.Text.Json.Serialization;

namespace metrics.Serialization.Tests.Models
{
    public class TestSerializationObject
    {
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Date { get; set; }
    }
}