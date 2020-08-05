namespace Base.Contracts
{
    public interface IUserTokenRemoved
    {
        int UserId { get; }
        string Name { get; }
    }

    public class UserTokenRemoved : IUserTokenRemoved
    {
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}