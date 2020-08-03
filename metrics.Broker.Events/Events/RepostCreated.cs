namespace metrics.Broker.Events.Events
{
    public interface IRepostCreated
    {
        int UserId { get; set; }
        int OwnerId { get; set; }
        int Id { get; set; }
    }
    public class RepostCreated : IRepostCreated
    {
        public int UserId { get; set; }
        public int OwnerId { get; set; }
        public int Id { get; set; }
    }
}