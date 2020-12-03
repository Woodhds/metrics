namespace metrics.Authentication.Infrastructure
{
    public interface IAuthenticatedUserProvider
    {
        IUser? GetUser();
    }
}