using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{

    public class NodeTestFixture : IDisposable
    {
        private static readonly AkkaConfiguration DefaultAkkaConfig = new AutoTestAkkaConfiguration();
        private readonly List<IDomainConfiguration> _domainConfigurations = new List<IDomainConfiguration>();
        private readonly List<IContainerConfiguration> _containerConfigurations = new List<IContainerConfiguration>();

        public NodeTestFixture(IDomainConfiguration domainConfiguration = null,
                               TimeSpan? defaultTimeout = null,
                               ITestOutputHelper helper = null)
        {
            if (domainConfiguration != null)
                Add(domainConfiguration);

            DefaultTimeout = defaultTimeout ?? DefaultTimeout;
            Output = helper;
            SystemConfigFactory = () => AkkaConfig.ToStandAloneInMemorySystemConfig();
            ActorSystemCreator = () => ActorSystem.Create(Name, SystemConfigFactory());
        }
        public NodeTestFixture(params IDomainConfiguration[] domainConfiguration):this()
        {
            foreach (var c in domainConfiguration)
                Add(c);
        }
        public NodeSettings NodeSettings { get; private set; }
        public GridDomainNode Node { get; private set; }
        public ActorSystem System { get; private set; }
        public Func<string> SystemConfigFactory { get; set; }
        public ILogger Logger { get; private set; }
        public AkkaConfiguration AkkaConfig { get; set; } = DefaultAkkaConfig;
        public string Name => AkkaConfig.Network.SystemName;
        internal TimeSpan DefaultTimeout { get; } = Debugger.IsAttached ? TimeSpan.FromHours(1) : TimeSpan.FromSeconds(3);
        public ITestOutputHelper Output { get; set; }
        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Verbose;

        public void Dispose()
        {
            Node.Stop().Wait();
        }

        public void Add(IDomainConfiguration config)
        {
            _domainConfigurations.Add(config);
        }
        public void Add(IContainerConfiguration config)
        {
            _containerConfigurations.Add(config);
        }

        public virtual async Task<GridDomainNode> CreateNode()
        {
            Logger = new XUnitAutoTestLoggerConfiguration(Output, LogLevel).CreateLogger();
            NodeSettings = CreateNodeSettings();

            OnNodePreparingEvent.Invoke(this, this);
            Node = new GridDomainNode(NodeSettings);
            Node.Initializing += (sender, node) => OnNodeCreatedEvent.Invoke(this, node);
            await Node.Start();
            OnNodeStartedEvent.Invoke(this, Node);

            return Node;
        }

        protected virtual NodeSettings CreateNodeSettings()
        {
            var settings = new NodeSettings(CreateSystem)
                           {
                               QuartzConfig = new InMemoryQuartzConfig(),
                               DefaultTimeout = DefaultTimeout,
                               Log = Logger,
                               CustomContainerConfiguration = new ContainerConfiguration(_containerConfigurations.ToArray())
                           };

            settings.DomainBuilder.Register(_domainConfigurations);

            return settings;
        }
        public Func<ActorSystem> ActorSystemCreator { get; set; }
        private ActorSystem CreateSystem()
        {
            if (System == null)
                System = ActorSystemCreator();

            ExtendedActorSystem actorSystem = (ExtendedActorSystem)System;

            var logActor = actorSystem.SystemActorOf(Props.Create(() => new SerilogLoggerActor(Logger)), "node-log-test");

            logActor.Ask<LoggerInitialized>(new InitializeLogger(actorSystem.EventStream)).Wait();
            return System;
        }

        public event EventHandler<GridDomainNode>  OnNodeStartedEvent   = delegate { };
        public event EventHandler<NodeTestFixture> OnNodePreparingEvent = delegate { };
        public event EventHandler<GridDomainNode>  OnNodeCreatedEvent   = delegate { };
    }
}