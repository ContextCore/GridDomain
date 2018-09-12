using System;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Configuration
{
    public interface IAggregateDependencies<TAggregate> : IRouteMapFactory where TAggregate : IAggregate
    {
        ISnapshotsPersistencePolicy SnapshotPolicy { get; }
        IAggregateFactory AggregateFactory { get; }
        ISnapshotFactory SnapshotFactory { get; }
        IRecycleConfiguration RecycleConfiguration { get; }
    }
}