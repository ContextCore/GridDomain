using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Node.Configuration.Composition {
    public class SagaStateDependencyFactory<TState> : DefaultAggregateDependencyFactory<SagaStateAggregate<TState>> where TState : ISagaState
    {
        public SagaStateDependencyFactory():base(() => new SagaStateCommandHandler<TState>(), () => EmptyRouteMap.Instance)
        {
        }
    }
}