using System.Collections.Generic;
using System.Runtime.InteropServices;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaCommandsBuilder<TMessage, TAggregate> :
        ICommandRouteBuilder<TMessage, TAggregate> where TAggregate : AggregateBase
    {
        private readonly IHandler<CreateActorRoute> _routingRegistrator;
        public string CorrelationPropertyName;
        private readonly IAggregateActorLocator _actorLocator;

        public AkkaCommandsBuilder(IAggregateActorLocator actorLocator, IHandler<CreateActorRoute> routingRegistrator)
        {
            _actorLocator = actorLocator;
            _routingRegistrator = routingRegistrator;
        }

        public void Register()
        {
            var aggregateActorType = _actorLocator.GetActorTypeFor<TAggregate>();

            _routingRegistrator.Handle(new CreateActorRoute(typeof(TMessage), aggregateActorType, typeof(TAggregate)));
        }

        public ICommandRouteBuilder<TMessage, TAggregate> WithCorrelation(string name)
        {
            CorrelationPropertyName = name;
            return this;
        }
    }
}