using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Node;
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
        private static readonly NodeConfiguration DefaultNodeConfig = new AutoTestNodeConfiguration(
#if DEBUG
             LogEventLevel.Debug
#else
             LogEventLevel.Warning
#endif
            );

        private readonly List<IDomainConfiguration> _domainConfigurations = new List<IDomainConfiguration>();

        public NodeTestFixture(ITestOutputHelper helper, IDomainConfiguration domainConfiguration) : this(helper, new[] {domainConfiguration})
        {
            
        }
       
        public NodeTestFixture(ITestOutputHelper helper, IDomainConfiguration[] domainConfiguration = null, TimeSpan? defaultTimeout = null)
        {
            DefaultTimeout = defaultTimeout ?? DefaultTimeout;
            SystemConfigFactory = () => NodeConfig.ToStandAloneInMemorySystemConfig();
            Logger = new XUnitAutoTestLoggerConfiguration(helper, NodeConfig.LogLevel).CreateLogger();
            if (domainConfiguration == null)
                return;

            foreach(var c in domainConfiguration)
                Add(c);
        }
        public GridDomainNode Node { get; private set; }
        public Func<string> SystemConfigFactory { get; set; }
        public ILogger Logger { get; }
        public NodeConfiguration NodeConfig { get; set; } = DefaultNodeConfig;
        public string Name => NodeConfig.Name;
        public LogEventLevel LogLevel { get => NodeConfig.LogLevel;
                                        set => NodeConfig.LogLevel = value;}
        private const int DefaultTimeOutSec =
#if DEBUG
            10; //in debug mode all messages serialization is enabled, and it slows down all tests greatly
#endif
#if !DEBUG
            3;
#endif
        private TimeSpan DefaultTimeout { get; } = Debugger.IsAttached ? TimeSpan.FromHours(1) : TimeSpan.FromSeconds(DefaultTimeOutSec);

        public void Dispose()
        {
            Node.Stop().Wait();
        }

        public NodeTestFixture Add(IDomainConfiguration config)
        {
            _domainConfigurations.Add(config);
            return this;
        }

        public async Task<GridDomainNode> CreateNode(Func<ActorSystem> actorSystemProvider = null)
        {
            OnNodePreparingEvent.Invoke(this, this);
            Node = new GridDomainNode(_domainConfigurations, new DelegateActorSystemFactory(actorSystemProvider ?? InitActorSystem), Logger, DefaultTimeout);
            Node.Initializing += (sender, node) => OnNodeCreatedEvent.Invoke(this, node);
            await Node.Start();
            OnNodeStartedEvent.Invoke(this, Node);

            return Node;
        }
      
        private ActorSystem InitActorSystem()
        {
            var system = NodeConfig.CreateInMemorySystem();
            system.AttachSerilogLogging(Logger);
            return system;
        }

        public event EventHandler<GridDomainNode>  OnNodeStartedEvent   = delegate { };
        public event EventHandler<NodeTestFixture> OnNodePreparingEvent = delegate { };
        public event EventHandler<GridDomainNode>  OnNodeCreatedEvent   = delegate { };
    }
}