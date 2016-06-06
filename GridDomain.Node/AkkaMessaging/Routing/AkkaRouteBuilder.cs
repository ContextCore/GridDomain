using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class AkkaRouteBuilder<T> : IRouteBuilder<T>
    {
        private readonly IAggregateActorLocator _actorLocator;
        private readonly IHandler<CreateActorRoute> _routingActorRegistrator;
        private readonly IHandler<CreateHandlerRoute> _routingRegistrator;

        public AkkaRouteBuilder(IHandler<CreateHandlerRoute> routingRegistrator,
            IHandler<CreateActorRoute> routingActorRegistrator,
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