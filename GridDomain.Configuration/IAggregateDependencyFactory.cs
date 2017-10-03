using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Configuration
{
    public interface IAggregateDependencyFactory<TAggregate> : IRouteMapFactory where TAggregate : Aggregate
    {
        IAggregateCommandsHandler<TAggregate> CreateCommandsHandler();
        ISnapshotsPersistencePolicy CreatePersistencePolicy();
        IConstructAggregates CreateAggregateFactory();
        IPersistentChildsRecycleConfiguration CreateRecycleConfiguration();
    }
}