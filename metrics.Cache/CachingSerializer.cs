using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using metrics.Cache.Abstractions;

namespace metrics.Cache
{
    public class CachingSerializer : ICachingSerializer
    {
        public async Task<T> DeserializeAsync<T>([NotNull]byte[] bytes, CancellationToken cancellationToken = default)
        {
            if (bytes == null || bytes.Length == 0)
                return default;

            await using var ms = new MemoryStream(bytes);
            
            return await JsonSerializer.DeserializeAsync<T>(ms, cancellationToken: cancellationToken);
        }

        public Task<byte[]> SerializeAsync<T>(T value, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(JsonSerializer.SerializeToUtf8Bytes(value));
        }
    }
}