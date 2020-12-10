﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace metrics.Serialization
{
    public class UnixDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return UnixEpoch.AddSeconds(reader.GetDouble());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(
                (new DateTime(
                     value.Year,
                     value.Month,
                     value.Day,
                     value.Hour,
                     value.Minute,
                     value.Second
                 )
                 - UnixEpoch).TotalSeconds);
        }
    }
}