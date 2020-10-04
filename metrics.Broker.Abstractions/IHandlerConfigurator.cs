namespace metrics.Broker.Abstractions
{
    public interface IHandlerConfigurator
    {
        void ConfigureConsumer<TEvent>() where TEvent : class, new();
        void ConfigureCommandConsumer<TEvent>() where TEvent : class, new();
        void ConfigureCommand<TCommand>(string host) where TCommand : class, new();
    }
}