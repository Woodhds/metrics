using System.IO;
using System.Linq;
using Google.Protobuf;

namespace metrics.Broker
{
    public interface IProtobufMessageSerializer
    {
        byte[] Serialize<T>(T data) where T : class, new();
        T Deserialize<T>(byte[] bytes) where T : class, new();
    }
    
    public class ProtobufMessageSerializer : IProtobufMessageSerializer
    {
        public byte[] Serialize<T>(T data) where T : class, new()
        {
            using var ms = new MemoryStream();
            ((IMessage)data).WriteTo(ms);
            return ms.ToArray();
        }

        public T Deserialize<T>(byte[] bytes) where T : class, new()
        {
            var message = new T();
            ((IMessage) message).MergeFrom(bytes.ToArray());

            return message;
        }
    }
}