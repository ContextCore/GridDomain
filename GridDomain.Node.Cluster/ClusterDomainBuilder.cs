using GridDomain.Configuration;
using GridDomain.Node.Cluster.CommandPipe;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Composition;
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

        protected override IContainerConfiguration CreateProcessManagerConfiguration<TState>(IProcessDependencyFactory<TState> processDependenciesfactory)
        {
            return new ProcessManagerConfiguration<TState,ClusterProcessActor<TState>>(processDependenciesfactory,_processManagersStateActorPath(typeof(TState)));
        }
    }
}