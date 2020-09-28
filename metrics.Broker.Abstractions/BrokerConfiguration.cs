namespace metrics.Broker.Abstractions
{
    public class BrokerConfiguration
    {
        public IMessageBroker MessageBroker;
        public IHandlerConfigurator HandlerConfigurator;
        public string Host;

        public BrokerConfiguration(IMessageBroker messageBroker, IHandlerConfigurator handlerConfigurator, string host)
        {
            MessageBroker = messageBroker;
            HandlerConfigurator = handlerConfigurator;
            Host = host;
        }
    }
}