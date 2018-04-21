using Akka.Actor;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node.Cluster.CommandPipe {
    public class ClusterProcessStateActor<TState> : ClusterAggregateActor<ProcessStateAggregate<TState>> where TState : IProcessState
    {
        public ClusterProcessStateActor(IAggregateCommandsHandler<ProcessStateAggregate<TState>> handler,
                                        ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                                        IConstructAggregates aggregateConstructor,
                                        IConstructSnapshots snapshotsConstructor,
                                        IActorRef customHandlersActor) : base(handler,
                                                                              snapshotsPersistencePolicy,
                                                                              aggregateConstructor,
                                                                              snapshotsConstructor,
                                                                              customHandlersActor)
        {
            int a;
        }

        protected override void AwaitingCommandBehavior()
        {
            Command<GetProcessState>(c => Sender.Tell(new ProcesStateMessage<TState>(State.State)));
            base.AwaitingCommandBehavior();
        }
    }
}