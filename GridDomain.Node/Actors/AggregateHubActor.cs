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

        protected override string GetChildActorName(Guid childId)
        {
            return AggregateActorName.New<TAggregate>(childId).ToString();
        }

        protected override Guid GetChildActorId(IMessageMetadataEnvelop message)
        {
            return (message.Message as ICommand)?.AggregateId ?? Guid.Empty;
        }

        protected override Type ChildActorType
        {
            get { return _actorType; }
        }
    }
}