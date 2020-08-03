namespace metrics.Broker.Events.Events
{
    public interface ILoginEvent
    {
        int UserId { get; set; }
        string Token { get; set; }
    }
    public class LoginEvent
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}