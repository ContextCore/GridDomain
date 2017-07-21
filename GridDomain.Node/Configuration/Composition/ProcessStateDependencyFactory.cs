using GridDomain.CQRS.Messaging;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node.Configuration.Composition {
    public class ProcessStateDependencyFactory<TState> : DefaultAggregateDependencyFactory<ProcessStateAggregate<TState>> where TState : IProcessState
    {
        public ProcessStateDependencyFactory():base(() => new ProcessStateCommandHandler<TState>(), () => EmptyRouteMap.Instance)
        {
        }
    }
}