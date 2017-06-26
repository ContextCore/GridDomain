using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Node.Configuration.Composition {
    public interface IDomainBuilder
    {
        void RegisterSaga<TState, TSaga>(ISagaDependencyFactory<TState, TSaga> factory) where TSaga : Process<TState>
                                                                                        where TState : class, ISagaState;

        void RegisterAggregate<TAggregate>(IAggregateDependencyFactory<TAggregate> factory) where TAggregate : Aggregate;
    }
}