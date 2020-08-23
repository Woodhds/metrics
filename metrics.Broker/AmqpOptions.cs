namespace metrics.Broker
{
    public class AmqpOptions
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public bool InMemory { get; set; }
    }
}