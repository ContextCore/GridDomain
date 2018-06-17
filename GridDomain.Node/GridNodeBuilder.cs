using System;
using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.Node.Configuration;
using Serilog;

namespace GridDomain.Node
{
    public class GridNodeBuilder
    {
        public ILogger Logger;
        public IDomainConfiguration[] Configurations;
        public TimeSpan DefaultTimeout;
        protected Func<ActorSystem> _actorProducers;

        protected Action<ActorSystem> _actorInit = delegate { };

        public GridNodeBuilder()
        {
            Logger = new DefaultLoggerConfiguration().CreateLogger()
                                                     .ForContext<GridDomainNode>();
            DefaultTimeout = TimeSpan.FromSeconds(10);
            Configurations = new IDomainConfiguration[] { };
        }

        public virtual IGridDomainNode Build()
        {
            var factory = new DelegateActorSystemFactory(_actorProducers, _actorInit);
            return new GridDomainLocalNode(Configurations, factory, Logger, DefaultTimeout);
        }

        public GridNodeBuilder ActorSystem(Func<ActorSystem> sys)
        {
            _actorProducers = sys;
            return this;
        }

        public GridNodeBuilder Initialize(Action<ActorSystem> sys)
        {
            _actorInit = sys;
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