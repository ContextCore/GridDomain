using GridDomain.Processes;
using GridDomain.Processes.Creation;
using GridDomain.Processes.State;

namespace GridDomain.Configuration {
    public interface IProcessManagerDependencyFactory<TState>: IRouteMapFactory where TState : class, IProcessState
    {
        IProcessManagerCreatorCatalog<TState> CreateCatalog();
        IAggregateDependencyFactory<ProcessStateAggregate<TState>> StateDependencyFactory { get; }
        string ProcessName { get; }
    }
}