namespace metrics.Broker.Abstractions
{
    public interface IHandlerConfigurator
    {
        void ConfigureConsumer<TEvent>() where TEvent : class;
        void ConfigureCommandConsumer<TEvent>() where TEvent : class;
        void ConfigureCommand<TCommand>(string host) where TCommand : class;
    }
}