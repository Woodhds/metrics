namespace metrics.Identity.Client.Abstractions
{
    public interface IUserTokenKeyProvider
    {
        string GetKey(int userId);
    }
}