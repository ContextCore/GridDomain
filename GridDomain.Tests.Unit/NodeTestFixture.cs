using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Node;
using GridDomain.Node.Actors.Serilog;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Configuration;
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
        public NodeTestFixture(IDomainConfiguration domainConfiguration = null, TimeSpan? defaultTimeout = null, ITestOutputHelper helper = null)
        {
            if (domainConfiguration != null)
                Add(domainConfiguration);

            DefaultTimeout = defaultTimeout ?? DefaultTimeout;
            Output = helper;
            SystemConfigFactory = () => AkkaConfig.ToDebugStandAloneInMemorySystemConfig();
            ActorSystemCreator = () => ActorSystem.Create(Name, SystemConfigFactory());
          
        }
        public NodeTestFixture(params IDomainConfiguration[] domainConfiguration)
        {
            foreach (var c in domainConfiguration)
                Add(c);
        }
        public GridDomainNode Node { get; private set; }
        public ActorSystem System { get; private set; }
        public Func<string> SystemConfigFactory { get; set; }
        public ILogger Logger { get; private set; }
        public AkkaConfiguration AkkaConfig { get; set; } = DefaultAkkaConfig;
        public string Name => AkkaConfig.Network.SystemName;

        private const int DefaultTimeOutSec =
#if DEBUG
            10; //in debug mode all messages serialization is enabled, and it slows down all tests greatly
#endif
#if !DEBUG
            3;
#endif      
        internal TimeSpan DefaultTimeout { get; } = Debugger.IsAttached ? TimeSpan.FromHours(1) : TimeSpan.FromSeconds(DefaultTimeOutSec);

        public ITestOutputHelper Output { get; set; }
        public LogEventLevel LogLevel { get; set; } =
#if DEBUG 
            LogEventLevel.Verbose;
#endif
#if !DEBUG
            LogEventLevel.Warning;
#endif

        public void Dispose()
        {
            Node.Stop().Wait();
        }

        public NodeTestFixture Add(IDomainConfiguration config)
        {
            _domainConfigurations.Add(config);
            return this;
        }

        public virtual async Task<GridDomainNode> CreateNode()
        {
            Logger = new XUnitAutoTestLoggerConfiguration(Output, LogLevel).CreateLogger();


            OnNodePreparingEvent.Invoke(this, this);
            Node = new GridDomainNode(_domainConfigurations, new DelegateActorSystemFactory(CreateSystem), Logger, DefaultTimeout);
            Node.Initializing += (sender, node) => OnNodeCreatedEvent.Invoke(this, node);
            await Node.Start();
            OnNodeStartedEvent.Invoke(this, Node);

            return Node;
        }

       
        public Func<ActorSystem> ActorSystemCreator { get; set; }
        private ActorSystem CreateSystem()
        {
            if (System == null)
                System = ActorSystemCreator();

            ExtendedActorSystem actorSystem = (ExtendedActorSystem)System;

            var logActor = actorSystem.SystemActorOf(Props.Create(() => new SerilogLoggerActor(new XUnitAutoTestLoggerConfiguration(Output, LogLevel).CreateLogger())), "node-log-test");
            logActor.Ask<LoggerInitialized>(new InitializeLogger(actorSystem.EventStream)).Wait();
            return System;
        }

        public event EventHandler<GridDomainNode>  OnNodeStartedEvent   = delegate { };
        public event EventHandler<NodeTestFixture> OnNodePreparingEvent = delegate { };
        public event EventHandler<GridDomainNode>  OnNodeCreatedEvent   = delegate { };
    }
}