namespace metrics.Broker.Abstractions
{
    public interface IHandlerConfigurator
    {
        void Configure<TEvent, THandler>() 
            where TEvent : class 
            where THandler : IMessageHandler<TEvent>;
    }
}