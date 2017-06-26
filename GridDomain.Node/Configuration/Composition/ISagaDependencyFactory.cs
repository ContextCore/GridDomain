using GridDomain.EventSourcing.CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Node.Configuration.Composition {
    public interface ISagaDependencyFactory<TState, TProces> where TState : class, ISagaState
                                                             where TProces : Process<TState>
    {
        ISaga—reatorCatalog<TState> CreateCatalog(string name);
        ISagaDescriptor CreateDescriptor(string name);
        IAggregateDependencyFactory<SagaStateAggregate<TState>> StateDependencyFactory { get; }
    }
}