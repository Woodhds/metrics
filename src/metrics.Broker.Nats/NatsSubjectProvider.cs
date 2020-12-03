namespace metrics.Broker.Nats
{
    public interface INatsSubjectProvider
    {
        string GetSubject<T>();
    }
    
    public class NatsSubjectProvider : INatsSubjectProvider
    {
        public string GetSubject<T>()
        {
            return typeof(T).FullName.ToLower() + "-subject";
        }
    }
}