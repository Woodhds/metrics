namespace metrics.Broker.Events.Events
{
    public class LoginEvent
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}