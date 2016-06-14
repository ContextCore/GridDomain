using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class AkkaRouteBuilder<T> : IRouteBuilder<T>
    {
        private readonly IAggregateActorLocator _actorLocator;
        private readonly IHandler<CreateActorRouteMessage> _routingActorRegistrator;
        private readonly IHandler<CreateHandlerRouteMessage> _routingRegistrator;

        public AkkaRouteBuilder(IHandler<CreateHandlerRouteMessage> routingRegistrator,
            IHandler<CreateActorRouteMessage> routingActorRegistrator,
            IAggregateActorLocator actorLocator)
        {
            _actorLocator = actorLocator;
            _routingActorRegistrator = routingActorRegistrator;
            _routingRegistrator = routingRegistrator;
        }

        public IHandlerBuilder<T, THandler> ToHandler<THandler>() where THandler : IHandler<T>
        {
            return new AkkaHandlerBuilder<T, THandler>(_routingRegistrator);
        }
    }
}