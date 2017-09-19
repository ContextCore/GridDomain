using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.PersistentHub;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors.Aggregates
{
    public class AggregateHubActor<TAggregate> : PersistentHubActor where TAggregate : EventSourcing.Aggregate
    {
        public AggregateHubActor(IPersistentChildsRecycleConfiguration conf) : base(conf, typeof(TAggregate).Name)
        {
            ChildActorType = typeof(AggregateActor<TAggregate>);
            Receive<CommandExecuted>(c => { }); 
            //TODO: can be awaited in message waiters
        }

        protected override void SendMessageToChild(ChildInfo knownChild, object message, IActorRef sender)
        {
            knownChild.Ref
                      .Ask<CommandExecuted>(message)
                      .ContinueWith(t => t.Result)
                      .PipeTo(sender);
        }

        protected override string GetChildActorName(Guid childId)
        {
            return AggregateActorName.New<TAggregate>(childId).ToString();
        }

        protected override Guid GetChildActorId(IMessageMetadataEnvelop message)
        {
            return (message.Message as ICommand)?.AggregateId ?? Guid.Empty;
        }

        protected override Type ChildActorType { get; }
    }
}