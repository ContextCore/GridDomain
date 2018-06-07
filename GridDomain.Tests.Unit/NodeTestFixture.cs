using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;
using GridDomain.Transport.Extension;
using Serilog;
using Serilog.Data;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{
    public class NodeTestFixture : IDisposable
    {
        public ITestOutputHelper Output { get; }

        private static readonly NodeConfiguration DefaultNodeConfig = new AutoTestNodeConfiguration();

        public readonly List<IDomainConfiguration> DomainConfigurations = new List<IDomainConfiguration>();

        public NodeTestFixture(ITestOutputHelper output, IDomainConfiguration domainConfiguration) : this(output, new[] {domainConfiguration}) { }

        public NodeTestFixture(ITestOutputHelper output,
                               IDomainConfiguration[] domainConfiguration = null,
                               TimeSpan? defaultTimeout = null) :
            this(output, null, null, domainConfiguration, defaultTimeout) { }

        public NodeTestFixture(ITestOutputHelper output,
                               NodeConfiguration cfg,
                               Func<NodeConfiguration, string> systemConfigFactorry = null,
                               IDomainConfiguration[] domainConfiguration = null,
                               TimeSpan? defaultTimeout = null)
        {
            Output = output;
            DefaultTimeout = defaultTimeout ?? DefaultTimeout;
            NodeConfig = cfg ?? DefaultNodeConfig;
            TestNodeBuilder = (node,kit) => new TestLocalNode(node, kit);
            ConfigBuilder = systemConfigFactorry ?? (n => n.ToStandAloneInMemorySystemConfig());
            NodeBuilder = BuildInMemoryStandAloneNode;
            
            if (domainConfiguration != null)
                foreach (var c in domainConfiguration)
                    Add(c);
        }

        public IExtendedGridDomainNode Node { get; private set; }
        public Func<NodeConfiguration, string> ConfigBuilder { get; set; }
        public Func<Func<ActorSystem>, ILogger, IExtendedGridDomainNode> NodeBuilder { get; set; }
        public NodeConfiguration NodeConfig { get; }
        public string Name => NodeConfig.Name;

        private const int DefaultTimeOutSec =
#if DEBUG
            10; //in debug mode all messages serialization is enabled, and it slows down all tests greatly
#endif
#if !DEBUG
            3;
#endif
        public TimeSpan DefaultTimeout { get; } = Debugger.IsAttached ? TimeSpan.FromHours(1) : TimeSpan.FromSeconds(DefaultTimeOutSec);
        public Func<IExtendedGridDomainNode,TestKit,ITestGridDomainNode> TestNodeBuilder { get; set; }

        public void Dispose()
        {
            Node.Stop()
                .Wait();
        }

        public NodeTestFixture Add(IDomainConfiguration config)
        {
            DomainConfigurations.Add(config);
            return this;
        }

        private async Task<IExtendedGridDomainNode> StartNode(IExtendedGridDomainNode node)
        {
            OnNodePreparingEvent.Invoke(this, this);
            Node = node;
            OnNodeCreatedEvent.Invoke(this, node);
            await Node.Start();
            OnNodeStartedEvent.Invoke(this, Node);

            return Node;
        }

        public Task<IExtendedGridDomainNode> CreateNode(ILogger logger = null)
        {
            return CreateNode(() => NodeConfig.CreateInMemorySystem(), logger ?? new XUnitAutoTestLoggerConfiguration(Output, NodeConfig.LogLevel).CreateLogger());
        }

        public Task<IExtendedGridDomainNode> CreateNode(Func<ActorSystem> actorSystemProvider, ILogger logger)
        {
            var gridDomainNode = NodeBuilder(actorSystemProvider, logger);
            
            return StartNode(gridDomainNode);
        }
        
        public ITestGridDomainNode CreateTestNode(IExtendedGridDomainNode node, TestKit kit)
        {
            return TestNodeBuilder(node, kit);
        }

        private GridDomainNode BuildInMemoryStandAloneNode(Func<ActorSystem> actorSystemProvider, ILogger logger)
        {
            var node = new GridNodeBuilder().ActorFactory(new DelegateActorSystemFactory(actorSystemProvider,
                                                                                        sys =>
                                                                                        {
                                                                                            sys.AttachSerilogLogging(logger);
                                                                                            sys.InitLocalTransportExtension();
                                                                                            OnSystemCreatedEvent.Invoke(this, sys);
                                                                                        }))
                                            .DomainConfigurations(DomainConfigurations.ToArray())
                                            .Log(logger)
                                            .Timeout(DefaultTimeout)
                                            .Build();
            return (GridDomainNode)node;
        }

        public event EventHandler<IExtendedGridDomainNode> OnNodeStartedEvent = delegate { };
        public event EventHandler<NodeTestFixture> OnNodePreparingEvent = delegate { };
        public event EventHandler<IExtendedGridDomainNode> OnNodeCreatedEvent = delegate { };
        public event EventHandler<ActorSystem> OnSystemCreatedEvent = delegate { };
    }
}