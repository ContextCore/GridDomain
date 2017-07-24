using System;
using System.Collections.Generic;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Logging;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Composition;
using Serilog;

namespace GridDomain.Node
{
    public class NodeSettings
    {
        public NodeSettings(Func<ActorSystem> actorSystemFactory)
        {
            ActorSystemFactory = actorSystemFactory ?? ActorSystemFactory;
        }

        public IReadOnlyCollection<IDomainConfiguration> DomainConfigurations => _domainConfigurations;
        private readonly List<IDomainConfiguration> _domainConfigurations = new List<IDomainConfiguration>();

        public void Add(IDomainConfiguration conf)
        {
            _domainConfigurations.Add(conf);
        }
        public IContainerConfiguration ContainerConfiguration { get; set; } = new EmptyContainerConfiguration();
        public Func<ActorSystem> ActorSystemFactory { get; }

        public ILogger Log { get; set; } = new DefaultLoggerConfiguration().CreateLogger().ForContext<GridDomainNode>();

        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}