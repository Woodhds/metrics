namespace metrics.Services.Abstractions
{
    public interface IEventStorage
    {
        int AddEvents(string userId, int count);
        int GetCurrentCount(string userId);
    }
}