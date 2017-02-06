using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit
{
    public class NodeTestFixture : IDisposable
    {
        public static readonly AkkaConfiguration DefaultAkkaConfig = new AutoTestAkkaConfiguration();

        private GridDomainNode _node;

        public GridDomainNode Node => _node ?? CreateNode().Result;

        public ActorSystem System { get; set; }

        private AkkaConfiguration AkkaConfig { get; } = DefaultAkkaConfig;
        private bool ClearDataOnStart => !InMemory;
        private bool InMemory { get; } = true;
        public string Name => AkkaConfig.Network.SystemName;
        private TimeSpan DefaultTimeout { get; }
        public ITestOutputHelper Output { get; set; }
        private readonly List<IContainerConfiguration> _containerConfiguration = new List<IContainerConfiguration>();
        private readonly List<IMessageRouteMap> _routeMap = new List<IMessageRouteMap>();

        protected void Add(IMessageRouteMap map)
        {
            _routeMap.Add(map);
        }

        protected void Add(IContainerConfiguration config)
        {
            _containerConfiguration.Add(config);
        }

        public string GetConfig()
        {
            return InMemory ? AkkaConfig.ToStandAloneInMemorySystemConfig() : AkkaConfig.ToStandAloneSystemConfig();
        }

        public NodeTestFixture(IContainerConfiguration containerConfiguration = null,
                               IMessageRouteMap map = null,
                               TimeSpan? defaultTimeout = null)
        {
            if (map != null)
                Add(map);
            if (containerConfiguration != null)
                Add(containerConfiguration);

            DefaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(3);
        }

        private async Task<GridDomainNode> CreateNode()
        {
            if (ClearDataOnStart)
                TestDbTools.ClearData(DefaultAkkaConfig.Persistence);

            var quartzConfig = InMemory ? (IQuartzConfig) new InMemoryQuartzConfig() : new PersistedQuartzConfig();

            await CreateLogger();

            _node = new GridDomainNode(new CustomContainerConfiguration(_containerConfiguration.ToArray()),
                                       new CompositeRouteMap(_routeMap.ToArray()),
                                       () => new[] {System ?? ActorSystem.Create(Name, GetConfig())},
                                       quartzConfig,
                                       DefaultTimeout);
            OnNodeCreated();
            await _node.Start();
            OnNodeStarted();

            return _node;
        }

        private async Task CreateLogger()
        {
            var extSystem = (ExtendedActorSystem) System;
            var logger =
                extSystem.SystemActorOf(
                    Props.Create(
                        () => new SerilogLoggerActor(new XUnitAutoTestLoggerConfiguration(Output, LogEventLevel.Verbose))),
                    "node-log-test");

            await logger.Ask<LoggerInitialized>(new InitializeLogger(System.EventStream));
        }

        protected virtual void OnNodeCreated() {}
        protected virtual void OnNodeStarted() {}

        public void Dispose()
        {
            Node.Stop()
                .Wait(DefaultTimeout);
        }
    }
}