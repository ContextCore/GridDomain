using GridDomain.Configuration;
using GridDomain.Node.Cluster.CommandPipe;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node.Cluster {
    public class ClusterDomainBuilder : DomainBuilder
    {
        public ClusterDomainBuilder() : base(t => Known.Paths.ShardRegion(typeof(ProcessStateAggregate<>).MakeGenericType(t)))
        {
            
        }

        protected override IContainerConfiguration CreateAggregateConfiguration<TAggregate>(IAggregateDependencyFactory<TAggregate> factory)
        {
            return new AggregateConfiguration<ClusterAggregateActor<TAggregate>, TAggregate>(factory);
        }

        public override void RegisterProcessManager<TState>(IProcessDependencyFactory<TState> processDependenciesfactory)
        {
            var processManagerConfiguration = new ProcessManagerConfiguration<TState,ClusterProcessActor<TState>>(processDependenciesfactory,_processManagersStateActorPath(typeof(TState)));
            var processStateConfiguration = new AggregateConfiguration<ClusterProcessStateActor<TState>, ProcessStateAggregate<TState>>(processDependenciesfactory.StateDependencyFactory);
            
            _containerConfigurations.Add(processManagerConfiguration);
            _containerConfigurations.Add(processStateConfiguration);
            _maps.Add(processDependenciesfactory.CreateRouteMap());
            _maps.Add(processDependenciesfactory.StateDependencyFactory.CreateRouteMap());
        }
    }
}