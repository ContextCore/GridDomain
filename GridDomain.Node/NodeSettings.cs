using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Logging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Scheduling.Quartz.Retry;
using Serilog;

namespace GridDomain.Node
{
    public class NodeSettings
    {
        public NodeSettings(Func<ActorSystem> actorSystemFactory)
        {
            ActorSystemFactory = actorSystemFactory ?? ActorSystemFactory;
        }

        public IDomainBuilder DomainBuilder => Builder;

        internal readonly DomainBuilder Builder = new DomainBuilder();
        public IContainerConfiguration CustomContainerConfiguration { get; set; } = new EmptyContainerConfiguration();
        public Func<ActorSystem> ActorSystemFactory { get; }

        public ILogger Log { get; set; } = new DefaultLoggerConfiguration().CreateLogger().ForContext<GridDomainNode>();

        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(10);
        public IQuartzConfig QuartzConfig { get; set; } = new InMemoryQuartzConfig();
        public IRetrySettings QuartzJobRetrySettings { get; set; } = new InMemoryRetrySettings(5,
                                                                                               TimeSpan.FromMinutes(10),
                                                                                               new DefaultExceptionPolicy());
    }
}