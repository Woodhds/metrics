using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using metrics.Cache.Abstractions;

namespace metrics.Cache
{
    public class CachingSerializer : ICachingSerializer
    {
        public T Deserialize<T>([NotNull]byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return default;

            using var ms = new MemoryStream(bytes);
            
            return JsonSerializer.Deserialize<T>(bytes.AsSpan());
        }

        public byte[] Serialize<T>(T value)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value);
        }
    }
}