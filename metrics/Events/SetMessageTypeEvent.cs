namespace metrics.Events
{
    public interface ISetMessageTypeEvent
    {
        int MessageId { get; set; }
        int OwnerId { get; set; }
        int MessageCategory { get; set; }
    }
    
    public class SetMessageTypeEvent
    {
        public int MessageId { get; set; }
        public int OwnerId { get; set; }
        public int MessageCategory { get; set; }
    }
}