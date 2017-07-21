using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;
using GridDomain.Routing;

namespace GridDomain.Node.Configuration.Composition {
    public class ProcessStateDependencyFactory<TState> : DefaultAggregateDependencyFactory<ProcessStateAggregate<TState>> where TState : IProcessState
    {
        public ProcessStateDependencyFactory():base(() => new ProcessStateCommandHandler<TState>(), () => EmptyRouteMap.Instance)
        {
        }
    }
}