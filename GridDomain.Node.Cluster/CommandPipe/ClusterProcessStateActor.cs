using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node.Cluster.CommandPipe {
    public class ClusterProcessStateActor<TState> : ClusterAggregateActor<ProcessStateAggregate<TState>> where TState : IProcessState
    {
        public ClusterProcessStateActor(
                                        ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                                        IAggregateFactory aggregateConstructor,
                                        ISnapshotFactory snapshotFactoryConstructor,
                                        IActorRef customHandlersActor,
                                        IRecycleConfiguration recycle) : base(
                                                                              snapshotsPersistencePolicy,
                                                                              aggregateConstructor,
                                                                              snapshotFactoryConstructor,
                                                                              customHandlersActor,recycle)
        {
        }

        protected override void AwaitingCommandBehavior()
        {
            Command<GetProcessState>(c => Sender.Tell(new ProcesStateMessage<TState>(State.State)));
            base.AwaitingCommandBehavior();
        }
    }
}