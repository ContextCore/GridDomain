using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
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
using Serilog;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit
{

    public class NodeTestFixture : IDisposable
    {
        private static readonly AkkaConfiguration DefaultAkkaConfig = new AutoTestAkkaConfiguration();
        public GridDomainNode Node { get; private set; }
        public ActorSystem System { get; set; }
        protected ILogger Logger { get; set; }
        private AkkaConfiguration AkkaConfig { get; } = DefaultAkkaConfig;
        private bool ClearDataOnStart => !InMemory;
        public bool InMemory { get; set; } = true;
        public string Name => AkkaConfig.Network.SystemName;
        private TimeSpan DefaultTimeout { get; } = Debugger.IsAttached ? TimeSpan.FromHours(1) : TimeSpan.FromSeconds(3);
        public ITestOutputHelper Output { get; set; }

        private readonly List<IContainerConfiguration> _containerConfiguration = new List<IContainerConfiguration>();
        private readonly List<IMessageRouteMap> _routeMap = new List<IMessageRouteMap>();
        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Verbose;

        public virtual LoggerConfiguration CreateLoggerConfiguration(ITestOutputHelper helper)
        {
          return new XUnitAutoTestLoggerConfiguration(helper, LogLevel);
        } 
     
        public void Add(IMessageRouteMap map)
        {
            _routeMap.Add(map);
        }

        public void Add(IContainerConfiguration config)
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

            DefaultTimeout = defaultTimeout ?? DefaultTimeout;

        }

        public async Task<GridDomainNode> CreateNode()
        {
            if (ClearDataOnStart)
                await TestDbTools.ClearData(DefaultAkkaConfig.Persistence);

            await CreateLogger();

            var settings = CreateNodeSettings();

            Node = new GridDomainNode(settings);
            OnNodeCreated();
            await Node.Start();
            OnNodeStarted();

            return Node;
        }

        protected virtual NodeSettings CreateNodeSettings()
        {
            var settings = new NodeSettings(new CustomContainerConfiguration(_containerConfiguration.ToArray()),
                new CompositeRouteMap(_routeMap.ToArray()),
                () => new[] {System ?? ActorSystem.Create(Name, GetConfig())})
                           {
                               QuartzConfig =  InMemory ? (IQuartzConfig)new InMemoryQuartzConfig() : new PersistedQuartzConfig(),
                               DefaultTimeout = DefaultTimeout,
                               Log = Logger
                           };

            return settings;
        }

        private async Task CreateLogger()
        {
            Logger = CreateLoggerConfiguration(Output).CreateLogger();

            var extSystem = (ExtendedActorSystem)System;
            var logActor =
                extSystem.SystemActorOf(
                    Props.Create(() => new SerilogLoggerActor(Logger)),
                    "node-log-test");

            await logActor.Ask<LoggerInitialized>(new InitializeLogger(System.EventStream));
        }

        protected virtual void OnNodeCreated() {}
        protected virtual void OnNodeStarted() {}

        public void Dispose()
        {
            Node.Stop().Wait();
        }
    }
}