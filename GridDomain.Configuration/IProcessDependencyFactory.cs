using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Configuration {
    public interface IProcessDependencyFactory<TState>: IRouteMapFactory where TState : class, IProcessState
    {
        IProcessStateFactory<TState> CreateStateFactory();
        IProcess<TState> CreateProcess();
        IRecycleConfiguration CreateRecycleConfiguration();
        IAggregateDependencies<ProcessStateAggregate<TState>> StateDependencies { get; }
        string ProcessName { get; }
    }
}