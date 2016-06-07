using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.DI.Core;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    //TODO: think about replace with ConsistentHashingPool
    public class AggregateHostActor<TAggregate> : UntypedActor
        where TAggregate : AggregateBase
    {
        private readonly IAggregateActorLocator _actorLocator;
        private readonly Type _actorType;
        private readonly IDictionary<Guid, IActorRef> _children = new Dictionary<Guid, IActorRef>();
        private readonly ICommandAggregateLocator<TAggregate> _locator;

        public AggregateHostActor(ICommandAggregateLocator<TAggregate> locator, IAggregateActorLocator actorLocator)
        {
            _actorLocator = actorLocator;
            _locator = locator;
            _actorType = _actorLocator.GetActorTypeFor<TAggregate>();
        }

        protected override void OnReceive(object message)
        {
            IActorRef knownChild;
            var command = (ICommand) message;
            var aggregateId = _locator.GetAggregateId(command);

            if (!_children.TryGetValue(aggregateId, out knownChild))
            {
                var props = Context.DI().Props(_actorType);
                var name = AggregateActorName.New<TAggregate>(aggregateId);
                knownChild = _children[aggregateId] = Context.ActorOf(props, name.ToString());
            }
            knownChild.Tell(message);
        }
    }
}