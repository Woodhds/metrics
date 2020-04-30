namespace metrics.Events
{
    public class SetMessageTypeEvent
    {
        public int MessageId { get; set; }
        public int OwnerId { get; set; }
        public int MessageCategory { get; set; }
    }
}