namespace metrics.Cache.Abstractions
{
    public interface ICachingSerializer
    {
        byte[] Serialize<T>(T value);
        T Deserialize<T>(byte[] bytes);
    }
}