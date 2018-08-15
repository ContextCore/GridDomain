using GridDomain.Configuration.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Configuration {
    public class ProcessStateDependencies<TState> : AggregateDependencies<ProcessStateAggregate<TState>> where TState : IProcessState
    {
        public ProcessStateDependencies():base(() => CommandAggregateHandler.New<ProcessStateAggregate<TState>>(), () => EmptyRouteMap.Instance)
        {
        }
    }
}