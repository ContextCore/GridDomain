using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Configuration
{
    public interface IAggregateDependencyFactory<TAggregate> : IRouteMapFactory where TAggregate : IAggregate
    {
        IAggregateCommandsHandler<TAggregate> CreateCommandsHandler();
        ISnapshotsPersistencePolicy CreatePersistencePolicy();
        IConstructAggregates CreateAggregateFactory();
        IConstructSnapshots  CreateSnapshotsFactory();
        IPersistentChildsRecycleConfiguration CreateRecycleConfiguration();
    }
}