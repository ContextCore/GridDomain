using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaHandlerBuilder<TMessage, THandler>:IHandlerBuilder<TMessage,THandler> where THandler : IHandler<TMessage>
    {
        private readonly ActorHandler<CreateRoute> _routingRegistrator;

        public AkkaHandlerBuilder(ActorHandler<CreateRoute> routingRegistrator)
        {
            _routingRegistrator = routingRegistrator;
        }

        public void Register()
        {
            _routingRegistrator.Handle(new CreateRoute() { HandlerType = typeof(THandler), MessageType = typeof(TMessage) });
        }
    }
}