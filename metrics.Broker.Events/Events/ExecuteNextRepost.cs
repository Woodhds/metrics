namespace metrics.Broker.Events.Events
{
    public interface IExecuteNextRepost
    {
        int UserId { get; }
    }
    public class ExecuteNextRepost : IExecuteNextRepost
    {
        public int UserId { get; set; }
    }
}