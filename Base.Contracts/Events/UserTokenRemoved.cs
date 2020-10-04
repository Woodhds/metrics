namespace Base.Contracts.Events
{
    public class UserTokenRemoved
    {
        public int UserId { get; set; }
        public string Name { get; set; } = "";
    }
}