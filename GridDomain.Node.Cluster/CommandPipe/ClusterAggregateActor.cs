using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.Aggregates;

namespace GridDomain.Node.Cluster.CommandPipe {
    public class ClusterAggregateActor<TAggregate> : AggregateActor<TAggregate> where TAggregate : class, IAggregate
    {
        public ClusterAggregateActor(IAggregateCommandsHandler<TAggregate> handler, ISnapshotsPersistencePolicy snapshotsPersistencePolicy, IConstructAggregates aggregateConstructor, IConstructSnapshots snapshotsConstructor, IActorRef customHandlersActor) : base(handler, snapshotsPersistencePolicy, aggregateConstructor, snapshotsConstructor, customHandlersActor) { }


        protected override void Project(IMessageMetadata metadata, DomainEvent[] events)
        {
            foreach(var evt in events)
                Project(new ShardedProcessMessageMetadataEnvelop(evt, evt.ProcessId, metadata));
        }
        
        protected override void Project(IMessageMetadata metadata, IFault fault)
        {
            Project(new ShardedProcessMessageMetadataEnvelop(fault, fault.ProcessId, metadata));
        }
    }
}