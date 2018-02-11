using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Node.Actors.PersistentHub;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors.Aggregates
{
    public class AggregateHubActor<TAggregate> : PersistentHubActor where TAggregate : EventSourcing.Aggregate
    {
        public AggregateHubActor(IRecycleConfiguration conf) : base(conf, typeof(TAggregate).Name)
        {
            ChildActorType = typeof(AggregateActor<TAggregate>);
        }

        protected override void SendMessageToChild(ChildInfo knownChild, object message, IActorRef sender)
        {
            knownChild.Ref.Tell(message, sender);
        }

        internal override string GetChildActorName(string childId)
        {
            return EntityActorName.New<TAggregate>(childId).ToString();
        }

        protected override string GetChildActorId(IMessageMetadataEnvelop message)
        {
            return (message.Message as ICommand)?.AggregateId;
        }

        protected override Type ChildActorType { get; }
    }
}                                           