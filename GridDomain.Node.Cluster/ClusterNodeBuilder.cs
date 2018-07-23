using System;
using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.Node;
using GridDomain.Node.Cluster;
using GridDomain.Node.Configuration;
using Serilog;

namespace GridDomain.Node.Cluster 
{
    public class ClusterNodeBuilder : IGridNodeBuilder
    {
        private readonly GridNodeBuilder _builder;
        public ClusterNodeBuilder(GridNodeBuilder builder = null)
        {
            _builder = builder ?? new GridNodeBuilder();
        }

        public IExtendedGridDomainNode Build()
        {
            var factory = new DelegateActorSystemFactory(_builder.ActorProducers, _builder.ActorInit);

            return new GridClusterNode(_builder.Configurations, factory, _builder.Logger, _builder.DefaultTimeout);
        }

        public IGridNodeBuilder ActorSystem(Func<ActorSystem> sys)
        {
            _builder.ActorSystem(sys);
            return this;
        }

        public IGridNodeBuilder Initialize(Action<ActorSystem> sys)
        {
            _builder.Initialize(sys);
            return this;
        }

        public IGridNodeBuilder Transport(Action<ActorSystem> sys)
        {
            _builder.Transport(sys);
            return this;
        }

        public IGridNodeBuilder Log(ILogger log)
        {
            _builder.Log(log);
            return this;
        }

        public ILogger Logger => _builder.Logger;

        public IGridNodeBuilder DomainConfigurations(params IDomainConfiguration[] domainConfigurations)
        {
            _builder.DomainConfigurations(domainConfigurations);
            return this;
        }

        public IGridNodeBuilder Timeout(TimeSpan timeout)
        {
            _builder.Timeout(timeout);
            return this;
        }
    }
}