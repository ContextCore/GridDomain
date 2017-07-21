using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Configuration {
    public interface IProcessManagerDependencyFactory<TState>: IRouteMapFactory where TState : class, IProcessState
    {
        IProcessManagerCreatorCatalog<TState> CreateCatalog();
        IAggregateDependencyFactory<ProcessStateAggregate<TState>> StateDependencyFactory { get; }
        string ProcessName { get; }
    }
}