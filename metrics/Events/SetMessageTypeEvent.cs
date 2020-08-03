namespace metrics.Events
{
    public interface ISetMessageTypeEvent
    {
        int MessageId { get; }
        int OwnerId { get; }
        int MessageCategory { get; }
    }
    
    public class SetMessageTypeEvent : ISetMessageTypeEvent
    {
        public int MessageId { get; set; }
        public int OwnerId { get; set; }
        public int MessageCategory { get; set; }
    }
}