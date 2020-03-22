using System.Threading;
using System.Threading.Tasks;

namespace metrics.Cache.Abstractions
{
    public interface ICachingSerializer
    {
        Task<byte[]> SerializeAsync<T>(T value, CancellationToken cancellationToken = default);
        Task<T> DeserializeAsync<T>(byte[] bytes, CancellationToken cancellationToken = default);
    }
}