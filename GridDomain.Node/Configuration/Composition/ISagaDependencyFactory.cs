using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Node.Configuration.Composition {
    public interface ISagaDependencyFactory<TProcess, TState>: IRouteMapFactory where TState : class, ISagaState
                                                              where TProcess : Process<TState>
    {
        ISagaCreatorCatalog<TState> CreateCatalog();
        IAggregateDependencyFactory<SagaStateAggregate<TState>> StateDependencyFactory { get; }
    }
}