using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
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
        public ProcessStateActor(IAggregateCommandsHandler<ProcessStateAggregate<TState>> handler,
                                 ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                                 IConstructAggregates aggregateConstructor,
                                 IConstructSnapshots snapshotsConstructor,
                                 IActorRef customHandlersActor) : base(handler,
                                                                       snapshotsPersistencePolicy,
                                                                       aggregateConstructor,
                                                                       snapshotsConstructor,
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