using System;
using System.Text.Json;
using Confluent.Kafka;

namespace metrics.Broker.Kafka
{
    public class KafkaJsonSerializer<T> : ISerializer<T>, IDeserializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data, typeof(T));
        }

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<T>(data);
        }
    }
}