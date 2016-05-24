using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaAggregateCommandsBuilder<TMessage, TAggregate, TAggregateActor> :
        IAggregateCommandRouteBuilder<TMessage, TAggregate> where TAggregate : AggregateBase
                                                            where TAggregateActor : AggregateActor<TAggregate>
    {
        private readonly IHandler<CreateActorRoute> _routingRegistrator;
        public string CorrelationPropertyName;
        public AkkaAggregateCommandsBuilder(IHandler<CreateActorRoute> routingRegistrator)
        {
            _routingRegistrator = routingRegistrator;
        }

        public void Register()
        {
            _routingRegistrator.Handle(CreateActorRoute.New<TMessage, TAggregate, TAggregateActor>());
        }

        public IAggregateCommandRouteBuilder<TMessage, TAggregate> WithCorrelation(string name)
        {
            CorrelationPropertyName = name;
            return this;
        }
    }
}