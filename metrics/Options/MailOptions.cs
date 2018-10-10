namespace metrics.Options
{
    public class MailOptions
    {
        public string From { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool RequireAuth { get; set; }
        public bool UseSSL { get; set; }
    }
}
