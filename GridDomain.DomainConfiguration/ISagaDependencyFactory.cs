using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Configuration {
    public interface ISagaDependencyFactory<TProcess, TState>: IRouteMapFactory where TState : class, ISagaState
                                                              where TProcess : Process<TState>
    {
        ISagaCreatorCatalog<TState> CreateCatalog();
        IAggregateDependencyFactory<SagaStateAggregate<TState>> StateDependencyFactory { get; }
    }
}