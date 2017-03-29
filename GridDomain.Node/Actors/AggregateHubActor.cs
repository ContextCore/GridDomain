using System;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    public class AggregateHubActor<TAggregate> : PersistentHubActor where TAggregate : Aggregate
    {
        private readonly Type _actorType;

        public AggregateHubActor(IPersistentChildsRecycleConfiguration conf) : base(conf, typeof(TAggregate).Name)
        {
            _actorType = typeof(AggregateActor<TAggregate>);
        }

        protected override string GetChildActorName(IMessageMetadataEnvelop message)
        {
            var command = message as ICommand;
            if (command != null)
                return AggregateActorName.New<TAggregate>(command.AggregateId).ToString();
            return null;
        }

        protected override Guid GetChildActorId(IMessageMetadataEnvelop message)
        {
            var command = message as ICommand;
            return command?.AggregateId ?? Guid.Empty;
        }

        protected override Type GetChildActorType(IMessageMetadataEnvelop message)
        {
            return _actorType;
        }
    }
}