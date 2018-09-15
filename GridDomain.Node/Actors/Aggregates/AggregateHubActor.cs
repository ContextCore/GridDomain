using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
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

        protected override void SendMessageToChild(IActorRef knownChild, object message, IActorRef sender)
        {
            knownChild.Tell(message, sender);
        }

        internal override string GetChildActorName(string childId)
        {
            return EntityActorName.New<TAggregate>(childId);
        }

        protected override string GetChildActorId(IMessageMetadataEnvelop message)
        {
            return (message.Message as ICommand)?.AggregateId;
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(ex =>
                                         {
                                             switch (ex)
                                             {
                                                     case CommandExecutionFailedException cf:
                                                         return Directive.Restart;
                                                     case CommandAlreadyExecutedException cae:
                                                         return Directive.Restart;
                                                     default:
                                                         return Directive.Stop;
                                             }
                                         });
        }

        protected override Type ChildActorType { get; }
    }
}                                           