namespace metrics.Services.Abstract
{
    public interface IEventStorage
    {
        int AddEvents(string userId, int count);
        int GetCurrentCount(string userId);
    }
}