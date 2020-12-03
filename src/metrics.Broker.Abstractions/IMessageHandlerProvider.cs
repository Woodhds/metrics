namespace metrics.Broker.Abstractions
{
    public interface IMessageHandlerProvider
    {
        void RegisterConsumer<TEvent, THandler>() where THandler : class, IMessageHandler<TEvent> where TEvent : class, new();
        void RegisterCommand<TCommand>() where TCommand : class, new();
        void RegisterCommandConsumer<TEvent, THandler>() where THandler : class, IMessageHandler<TEvent> where TEvent : class, new();
    }
}