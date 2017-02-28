using System;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    public class AggregateHubActor<TAggregate> : PersistentHubActor where TAggregate : AggregateBase
    {
        private readonly Type _actorType;

        public AggregateHubActor(IPersistentChildsRecycleConfiguration conf) : base(conf, typeof(TAggregate).Name)
        {
            _actorType = typeof(AggregateActor<TAggregate>);
        }

        protected override string GetChildActorName(object message)
        {
            var command = message as ICommand;
            if (command != null)
            {
                return AggregateActorName.New<TAggregate>(command.AggregateId)
                                         .ToString();
            }
            return null;
        }

        protected override Guid GetChildActorId(object message)
        {
            var command = message as ICommand;
            return command?.AggregateId ?? Guid.Empty;
        }

        protected override Type GetChildActorType(object message)
        {
            return _actorType;
        }
    }
}