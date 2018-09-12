using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node.Actors.ProcessManagers
{
    public class ProcessStateActor<TState> : AggregateActor<ProcessStateAggregate<TState>> where TState : IProcessState
    {
        public ProcessStateActor(
                                 ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                                 IAggregateFactory aggregateConstructor,
                                 ISnapshotFactory snapshotFactoryConstructor,
                                 IActorRef customHandlersActor) : base(
                                                                       snapshotsPersistencePolicy,
                                                                       aggregateConstructor,
                                                                       snapshotFactoryConstructor,
                                                                       customHandlersActor)
        {
        }

        protected override void AwaitingCommandBehavior()
        {
            Command<GetProcessState>(c => Sender.Tell(new ProcesStateMessage<TState>(State.State)));
            base.AwaitingCommandBehavior();
        }
      
    }
}