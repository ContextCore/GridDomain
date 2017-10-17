using Autofac;
using GridDomain.Configuration;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node.Configuration.Composition {
    class ProcessStateAggregateConfiguration<TState> : AggregateConfiguration<ProcessStateActor<TState>, ProcessStateAggregate<TState>> where TState : IProcessState
    {
        internal ProcessStateAggregateConfiguration(IAggregateDependencyFactory<ProcessStateAggregate<TState>> factory) : base(factory)
        {
        }

        protected override void RegisterHub(ContainerBuilder container)
        {
            container.Register<ProcessStateHubActor<TState>>(c => new ProcessStateHubActor<TState>(AggregateDependencyFactory.CreateRecycleConfiguration()));
        }
    }
}