using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;

namespace GridDomain.Node.Configuration.Composition {


    public interface IAggregateDependencyFactory<TAggregate> where TAggregate : Aggregate
    {
        IAggregateCommandsHandler<TAggregate> CreateCommandsHandler(string name);
        ISnapshotsPersistencePolicy CreatePersistencePolicy(string name);
        IConstructAggregates CreateFactory(string name);
    }
}