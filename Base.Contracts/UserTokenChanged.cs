namespace Base.Contracts
{
    public interface IUserTokenChanged
    {
        string Token { get; }
        int UserId { get; }
    }
    
    public class UserTokenChanged : IUserTokenChanged
    {
        public string Token { get; set; }
        public int UserId { get; set; }
    }
}