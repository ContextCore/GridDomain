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
             LogEventLevel.Info
#endif
            );

        private readonly List<IDomainConfiguration> _domainConfigurations = new List<IDomainConfiguration>();

        public NodeTestFixture(ITestOutputHelper helper, IDomainConfiguration domainConfiguration) : this(helper, new[] {domainConfiguration})
        {
            
        }
       
        public NodeTestFixture(ITestOutputHelper helper, 
                               IDomainConfiguration[] domainConfiguration = null, 
                               TimeSpan? defaultTimeout = null) :
            this(helper,null,null,domainConfiguration,defaultTimeout)
                 {
                    
                 }
        
        public NodeTestFixture(ITestOutputHelper helper, 
                               NodeConfiguration cfg, 
                               Func<NodeConfiguration, string> systemConfigFactorry=null, 
                               IDomainConfiguration[] domainConfiguration = null, 
                               TimeSpan? defaultTimeout = null)
        {
            DefaultTimeout = defaultTimeout ?? DefaultTimeout;
            NodeConfig = cfg ?? DefaultNodeConfig;
            
            ConfigBuilder =  systemConfigFactorry ?? (n => n.ToStandAloneInMemorySystemConfig());
            SystemConfig = new Lazy<string>(() => ConfigBuilder(NodeConfig));
            Logger = new XUnitAutoTestLoggerConfiguration(helper, NodeConfig.LogLevel).CreateLogger();
            if (domainConfiguration == null)
                return;
         
            foreach(var c in domainConfiguration)
                Add(c);
        }
        public GridDomainNode Node { get; private set; }
        public Func<NodeConfiguration,string> ConfigBuilder { get; set; }
        public Lazy<string> SystemConfig { get; }
        public ILogger Logger { get; }
        public NodeConfiguration NodeConfig { get; }
        public string Name => NodeConfig.Name;

      
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