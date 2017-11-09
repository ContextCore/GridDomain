using GridDomain.Configuration.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Configuration {
    public class ProcessStateDependencyFactory<TState> : DefaultAggregateDependencyFactory<ProcessStateAggregate<TState>> where TState : IProcessState
    {
        public ProcessStateDependencyFactory():base(() => CommandAggregateHandler.New<ProcessStateAggregate<TState>>(), () => EmptyRouteMap.Instance)
        {
        }
    }
}