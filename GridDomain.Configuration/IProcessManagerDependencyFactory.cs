using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Configuration {
    public interface IProcessDependencyFactory<TState>: IRouteMapFactory where TState : class, IProcessState
    {
        IProcessStateFactory<TState> CreateStateFactory();
        IProcess<TState> CreateProcess();
        IAggregateDependencyFactory<ProcessStateAggregate<TState>> StateDependencyFactory { get; }
        string ProcessName { get; }
    }
}