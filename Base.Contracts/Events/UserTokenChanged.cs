namespace Base.Contracts.Events
{
    public class UserTokenChanged
    {
        public string Token { get; set; } = "";
        public int UserId { get; set; }
    }
}