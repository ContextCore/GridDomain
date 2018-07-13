using System;
using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.Node.Configuration;
using Serilog;

namespace GridDomain.Node
{
    public class GridNodeBuilder : IGridNodeBuilder
    {
        public ILogger Logger { get; private set; }
        public IDomainConfiguration[] Configurations;
        public TimeSpan DefaultTimeout;
        public Func<ActorSystem> ActorProducers;

        public Action<ActorSystem> ActorInit = delegate { };
        private Action<ActorSystem> _transport;

        public GridNodeBuilder(ILogger log)
        {
            Logger = log;
            DefaultTimeout = TimeSpan.FromSeconds(10);
            Configurations = new IDomainConfiguration[] { };
        }

        public virtual IGridDomainNode Build()
        {
            var factory = new DelegateActorSystemFactory(ActorProducers, _transport+ActorInit);
            return new GridDomainLocalNode(Configurations, factory, Logger, DefaultTimeout);
        }

        public GridNodeBuilder ActorSystem(Func<ActorSystem> sys)
        {
            ActorProducers = sys;
            return this;
        }
     
        public GridNodeBuilder Initialize(Action<ActorSystem> sys)
        {
            ActorInit += sys;
            return this;
        }

        public GridNodeBuilder Transport(Action<ActorSystem> sys)
        {
            _transport = sys;
            return this;
        }

        public GridNodeBuilder Log(ILogger log)
        {
            Logger = log;
            return this;
        }

        public GridNodeBuilder DomainConfigurations(params IDomainConfiguration[] domainConfigurations)
        {
            Configurations = domainConfigurations;
            return this;
        }

        public GridNodeBuilder Timeout(TimeSpan timeout)
        {
            DefaultTimeout = timeout;
            return this;
        }
    }
}