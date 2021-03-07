namespace metrics.Broker.Redis
{
    public interface IRedisMessageSerializer
    {
        byte[] Serialize<T>(T data) where T : class, new();
        T Deserialize<T>(byte[] bytes) where T : class, new();
    }

    public class RedisMessageSerializer : IRedisMessageSerializer
    {
        private readonly IProtobufMessageSerializer _protobufMessageSerializer;

        public RedisMessageSerializer(IProtobufMessageSerializer protobufMessageSerializer)
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