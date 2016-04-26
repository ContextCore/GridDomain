using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaRouteBuilder<T> : IRouteBuilder<T>
    {
        private readonly TypedMessageActor<CreateRoute> _routingRegistrator;

        public AkkaRouteBuilder(TypedMessageActor<CreateRoute> routingRegistrator)
        {
            _routingRegistrator = routingRegistrator;
        }

        public IHandlerBuilder<T, THandler> To<THandler>() where THandler : IHandler<T>
        {
            return new AkkaHandlerBuilder<T, THandler>(_routingRegistrator);
        }
    }
}