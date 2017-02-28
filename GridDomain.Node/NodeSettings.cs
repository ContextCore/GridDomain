using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Logging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Retry;
using Serilog;

namespace GridDomain.Node
{
    public class NodeSettings
    {
        public NodeSettings(IContainerConfiguration configuration = null,
                            IMessageRouteMap messageRouting = null,
                            Func<ActorSystem[]> actorSystemFactory = null)
        {
            ActorSystemFactory = actorSystemFactory ?? ActorSystemFactory;
            MessageRouting = messageRouting ?? MessageRouting;
            Configuration = configuration ?? Configuration;
        }

        public IContainerConfiguration Configuration { get; } = new EmptyContainerConfiguration();
        public IMessageRouteMap MessageRouting { get; } = new EmptyRouteMap();
        public Func<ActorSystem[]> ActorSystemFactory { get; } = () => new[] {ActorSystem.Create("defaultSystem")};

        public ILogger Log { get; set; } = new DefaultLoggerConfiguration().CreateLogger().ForContext<GridDomainNode>();

        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(10);
        public IQuartzConfig QuartzConfig { get; set; } = new InMemoryQuartzConfig();

        public IRetrySettings QuartzJobRetrySettings { get; set; } = new InMemoryRetrySettings(5,
            TimeSpan.FromMinutes(10),
            new DefaultExceptionPolicy());
    }
}