using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class AkkaRouteBuilder<T> : IRouteBuilder<T>
    {
        private readonly IHandler<CreateHandlerRouteMessage> _routingRegistrator;

        public AkkaRouteBuilder(IHandler<CreateHandlerRouteMessage> routingRegistrator)
        {
            _routingRegistrator = routingRegistrator;
        }

        public IHandlerBuilder<T, THandler> ToHandler<THandler>() where THandler : IHandler<T>
        {
            return new AkkaHandlerBuilder<T, THandler>(_routingRegistrator);
        }
    }
}