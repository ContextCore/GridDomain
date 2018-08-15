using System;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Configuration
{
    public interface IAggregateDependencies<TAggregate> : IRouteMapFactory where TAggregate : IAggregate
    {
        IAggregateCommandsHandler<TAggregate> CreateCommandsHandler();
        ISnapshotsPersistencePolicy CreatePersistencePolicy();
        IAggregateFactory CreateAggregateFactory();
        IConstructSnapshots  CreateSnapshotsFactory();
        IRecycleConfiguration CreateRecycleConfiguration();
    }
}