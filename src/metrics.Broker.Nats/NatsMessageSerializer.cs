﻿namespace metrics.Broker.Nats
{
    public interface INatsMessageSerializer
    {
        byte[] Serialize<T>(T data) where T : class, new();
        T Deserialize<T>(byte[] bytes) where T : class, new();
    }
    
    public class NatsMessageSerializer : INatsMessageSerializer
    {
        private readonly IProtobufMessageSerializer _protobufMessageSerializer;

        public NatsMessageSerializer(IProtobufMessageSerializer protobufMessageSerializer)
        {
            _protobufMessageSerializer = protobufMessageSerializer;
        }

        public byte[] Serialize<T>(T data) where T : class, new()
        {
            return _protobufMessageSerializer.Serialize(data);
        }

        public T Deserialize<T>(byte[] bytes) where T : class, new()
        {
            return _protobufMessageSerializer.Deserialize<T>(bytes);
        }
    }
}