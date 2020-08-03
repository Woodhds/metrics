namespace metrics.Broker.Events.Events
{
    public interface ILoginEvent
    {
        int UserId { get; }
        string Token { get; }
    }
    public class LoginEvent : ILoginEvent
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}