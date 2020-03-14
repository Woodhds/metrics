namespace metrics.Broker.Abstractions
{
    public interface IHandlerConfigurator
    {
        void Configure<TEvent>() where TEvent : class;
    }
}