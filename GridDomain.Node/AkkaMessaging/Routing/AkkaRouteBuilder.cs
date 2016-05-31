using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaRouteBuilder<T> : IRouteBuilder<T>
    {
        private readonly IHandler<CreateHandlerRoute> _routingRegistrator;
        private readonly IHandler<CreateActorRoute> _routingActorRegistrator;
        private readonly IAggregateActorLocator _actorLocator;

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