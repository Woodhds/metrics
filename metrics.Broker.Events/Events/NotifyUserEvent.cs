namespace metrics.Broker.Events.Events
{
    public interface INotifyUserEvent
    {
        int UserId { get; set; }
    }
    public class NotifyUserEvent : INotifyUserEvent
    {
        public int UserId { get; set; }
    }
}