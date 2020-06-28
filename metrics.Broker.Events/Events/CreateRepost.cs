namespace metrics.Broker.Events.Events
{
    public class CreateRepost
    {
        public int UserId { get; set; }
        public int OwnerId { get; set; }
        public int Id { get; set; }
    }
}