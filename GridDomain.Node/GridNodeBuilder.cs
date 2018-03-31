using System;
using GridDomain.Configuration;
using GridDomain.Node.Configuration;
using Serilog;

namespace GridDomain.Node
{
    public class GridNodeBuilder
    {
        public IActorSystemFactory ActorSystemFactory;
        public ILogger Logger;
        public IDomainConfiguration[] Configurations;
        public TimeSpan DefaultTimeout;

        public GridNodeBuilder()
        {
            Logger = new DefaultLoggerConfiguration().CreateLogger()
                                                     .ForContext<GridDomainNode>();
            DefaultTimeout = TimeSpan.FromSeconds(10);
            ActorSystemFactory = new HoconActorSystemFactory("system", "");
            Configurations = new IDomainConfiguration[] { };
        }

        public IGridDomainNode Build()
        {
            return new GridDomainLocalNode(Configurations, ActorSystemFactory, Logger, DefaultTimeout);
        }

        public GridNodeBuilder PipeFactory(IActorSystemFactory factory)
        {
            ActorSystemFactory = factory;
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
            this.DefaultTimeout = timeout;
            return this;
        }
    }
}