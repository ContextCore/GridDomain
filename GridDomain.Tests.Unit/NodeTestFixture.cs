using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
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
using Serilog.Core;
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
        public readonly Logger DefaultLogger;

        public NodeTestFixture(ITestOutputHelper output, IDomainConfiguration domainConfiguration) : this(output, new[] {domainConfiguration}) { }

        public NodeTestFixture(ITestOutputHelper output,
                               IDomainConfiguration[] domainConfiguration = null,
                               TimeSpan? defaultTimeout = null) :
            this(output, null, domainConfiguration, defaultTimeout) { }

        public NodeTestFixture(ITestOutputHelper output,
                               NodeConfiguration cfg,
                               IDomainConfiguration[] domainConfiguration = null,
                               TimeSpan? defaultTimeout = null)
        {
            Output = output;
            DefaultTimeout = defaultTimeout ?? DefaultTimeout;
            NodeConfig = cfg ?? DefaultNodeConfig;
            DefaultLogger = new XUnitAutoTestLoggerConfiguration(Output, NodeConfig.LogLevel).CreateLogger();
            ActorSystemConfigBuilder = new ActorSystemConfigBuilder(DefaultLogger);

            NodeConfig.ConfigureStandAloneInMemorySystem(ActorSystemConfigBuilder, true);

            TestNodeBuilder = (node, kit) => new TestLocalNode(node, kit);

            NodeBuilder = new GridNodeBuilder(); // (actorSystemProvider, logger) => BuildInMemoryStandAloneNode(actorSystemProvider, logger, new GridNodeBuilder());
            NodeBuilderConfigurator = ConfigureNodeBuilder;
            if (domainConfiguration != null)
                foreach (var c in domainConfiguration)
                    Add(c);
        }

        public IActorSystemConfigBuilder ActorSystemConfigBuilder { get; }
        public GridNodeBuilder NodeBuilder { get; set; }
        public Action<Func<ActorSystem>, ILogger, GridNodeBuilder> NodeBuilderConfigurator { get; set; }
        public IExtendedGridDomainNode Node { get; private set; }
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
        public Func<IExtendedGridDomainNode, TestKit, ITestGridDomainNode> TestNodeBuilder { get; set; }

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
            return CreateNode(() => CreateActorSystem(NodeConfig), logger ?? DefaultLogger);
        }

        protected virtual ActorSystem CreateActorSystem(NodeConfiguration cfg)
        {
            return cfg.CreateInMemorySystem();
        }

        public Task<IExtendedGridDomainNode> CreateNode(Func<ActorSystem> actorSystemProvider, ILogger logger)
        {
            ConfigureNodeBuilder(actorSystemProvider, logger, NodeBuilder);
            var node = NodeBuilder.Build();

            var gridDomainNode = (GridDomainNode) node;

            return StartNode(gridDomainNode);
        }

        private void ConfigureNodeBuilder(Func<ActorSystem> actorSystemProvider, ILogger logger, GridNodeBuilder gridNodeBuilder)
        {
            gridNodeBuilder.ActorSystem(actorSystemProvider)
                           .Initialize(sys =>
                                       {
                                           sys.AttachSerilogLogging(logger);
                                           sys.InitLocalTransportExtension();
                                       })
                           .DomainConfigurations(DomainConfigurations.ToArray())
                           .Log(logger)
                           .Timeout(DefaultTimeout);
        }

        public ITestGridDomainNode CreateTestNode(IExtendedGridDomainNode node, TestKit kit)
        {
            return TestNodeBuilder(node, kit);
        }

        public event EventHandler<IExtendedGridDomainNode> OnNodeStartedEvent = delegate { };
        public event EventHandler<NodeTestFixture> OnNodePreparingEvent = delegate { };
        public event EventHandler<IExtendedGridDomainNode> OnNodeCreatedEvent = delegate { };
    }
}