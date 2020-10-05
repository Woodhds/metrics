using System;
using System.IO;
using Confluent.Kafka;
using Google.Protobuf;

namespace metrics.Broker.Kafka
{
    public class KafkaProtobufSerializer<T> : ISerializer<T>, IDeserializer<T> where T : new()
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            using var ms = new MemoryStream();
            ((IMessage)data).WriteTo(ms);
            return ms.ToArray();
        }

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var message = new T();
            ((IMessage) message).MergeFrom(data.ToArray());

            return message;
        }
    }
}