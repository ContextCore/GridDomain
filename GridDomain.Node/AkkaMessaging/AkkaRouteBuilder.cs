using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaRouteBuilder<T> : IRouteBuilder<T>
    {
        private readonly IHandler<CreateRoute> _routingRegistrator;
        private readonly IHandler<CreateActorRoute> _routingActorRegistrator;

        public AkkaRouteBuilder(IHandler<CreateRoute> routingRegistrator,
                                IHandler<CreateActorRoute> routingActorRegistrator )
        {
            _routingActorRegistrator = routingActorRegistrator;
            _routingRegistrator = routingRegistrator;
        }

        public IHandlerBuilder<T, THandler> ToHandler<THandler>() where THandler : IHandler<T>
        {
            return new AkkaHandlerBuilder<T, THandler>(_routingRegistrator);
        }

        public IAggregateCommandRouteBuilder<T, TAggregate> ToAggregate<TAggregate>() 
            where TAggregate : AggregateBase 
        {
            return new AkkaAggregateCommandsBuilder<T,TAggregate, TAggregateActor>(_routingActorRegistrator);
        }
    }
}