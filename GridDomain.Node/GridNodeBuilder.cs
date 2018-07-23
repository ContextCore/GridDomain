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
        private Action<ActorSystem> _transport = delegate {};

        public GridNodeBuilder()
        {
            DefaultTimeout = TimeSpan.FromSeconds(10);
            Configurations = new IDomainConfiguration[] { };
        }

        public virtual IExtendedGridDomainNode Build()
        {
            if(Logger == null)
                throw new InvalidOperationException("Configure Logger before building grid node");

            var factory = new DelegateActorSystemFactory(ActorProducers, _transport+ActorInit);
            return new GridDomainLocalNode(Configurations, factory, Logger, DefaultTimeout);
        }

        public IGridNodeBuilder ActorSystem(Func<ActorSystem> sys)
        {
            ActorProducers = sys;
            return this;
        }
     
        public IGridNodeBuilder Initialize(Action<ActorSystem> sys)
        {
            ActorInit += sys;
            return this;
        }

        //TODO:remove in favor of ActorSystemBuilder?
        public IGridNodeBuilder Transport(Action<ActorSystem> sys)
        {
            _transport = sys;
            return this;
        }

        //TODO:remove in favor of ActorSystemBuilder?
        public IGridNodeBuilder Log(ILogger log)
        {
            Logger = log;
            return this;
        }

        public IGridNodeBuilder DomainConfigurations(params IDomainConfiguration[] domainConfigurations)
        {
            Configurations = domainConfigurations;
            return this;
        }

        public IGridNodeBuilder Timeout(TimeSpan timeout)
        {
            DefaultTimeout = timeout;
            return this;
        }
    }
}