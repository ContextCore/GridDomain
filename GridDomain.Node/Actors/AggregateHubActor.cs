using System;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    public class AggregateHubActor<TAggregate> : PersistentHubActor where TAggregate : AggregateBase
    {
        private readonly ICommandAggregateLocator<TAggregate> _locator;
        private readonly Type _actorType;

        public AggregateHubActor(ICommandAggregateLocator<TAggregate> locator)
        {
            _actorType = typeof(AggregateActor<TAggregate>);
            _locator = locator;
        }

        protected override string GetChildActorName(object message)
        {
            if (message is ICommand)
            {
                return AggregateActorName.New<TAggregate>(_locator.GetAggregateId(message as ICommand)).ToString();
            }
            return null;
        }

        protected override Guid GetChildActorId(object message)
        {
            if (message is ICommand)
            {
                return _locator.GetAggregateId(message as ICommand);
            }
            return Guid.Empty;
        }

        protected override Type GetChildActorType(object message)
        {
            return _actorType;
        }
    }
}