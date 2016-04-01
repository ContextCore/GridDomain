using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.CQRS.Messaging.MessageRouting.InMemoryRouting;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaRouteBuilder<T> : IRouteBuilder<T>
    {
        private readonly ActorHandler<CreateRoute> _routingRegistrator;

        public AkkaRouteBuilder(ActorHandler<CreateRoute> routingRegistrator)
        {
            _routingRegistrator = routingRegistrator;
        }

        public IHandlerBuilder<T, THandler> To<THandler>() where THandler : IHandler<T>
        {
            return new AkkaHandlerBuilder<T, THandler>(_routingRegistrator);
        }
    }
}