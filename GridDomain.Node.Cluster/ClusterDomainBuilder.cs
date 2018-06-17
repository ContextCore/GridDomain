using System.Linq;
using Autofac;
using Autofac.Core;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Cluster.CommandPipe;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node.Cluster
{
    public class ClusterAggregateConfiguration<TAggregate> : AggregateConfiguration<ClusterAggregateActor<TAggregate>, TAggregate> where TAggregate : Aggregate
    {
        public ClusterAggregateConfiguration(IAggregateDependencyFactory<TAggregate> factory) : base(factory) { }

        protected override Parameter[] CreateParametersRegistration()
        {
            return base.CreateParametersRegistration()
                       .Union(new[]
                              {
                                  new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IRecycleConfiguration),
                                                        (pi, ctx) => AggregateDependencyFactory.CreateRecycleConfiguration())
                              })
                       .ToArray();
        }
    }

    public class ClusterDomainBuilder : DomainBuilder
    {
        public ClusterDomainBuilder() : base(t => Known.Paths.ShardRegion(typeof(ProcessStateAggregate<>).MakeGenericType(t))) { }

        protected override IContainerConfiguration CreateAggregateConfiguration<TAggregate>(IAggregateDependencyFactory<TAggregate> factory)
        {
            return new ClusterAggregateConfiguration<TAggregate>(factory);
        }

        public override void RegisterProcessManager<TState>(IProcessDependencyFactory<TState> processDependenciesfactory)
        {
            var processManagerConfiguration = new ProcessManagerConfiguration<TState, ClusterProcessActor<TState>>(processDependenciesfactory, _processManagersStateActorPath(typeof(TState)));
            var processStateConfiguration = new AggregateConfiguration<ClusterProcessStateActor<TState>, ProcessStateAggregate<TState>>(processDependenciesfactory.StateDependencyFactory);

            _containerConfigurations.Add(processManagerConfiguration);
            _containerConfigurations.Add(processStateConfiguration);
            _maps.Add(processDependenciesfactory.CreateRouteMap());
            _maps.Add(processDependenciesfactory.StateDependencyFactory.CreateRouteMap());
        }
    }
}