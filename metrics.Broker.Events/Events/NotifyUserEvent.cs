namespace metrics.Broker.Events.Events
{
    public interface INotifyUserEvent
    {
        int UserId { get; }
    }
    public class NotifyUserEvent : INotifyUserEvent
    {
        public int UserId { get; set; }
    }
}