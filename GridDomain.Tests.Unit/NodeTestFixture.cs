using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.Xunit2;
using Castle.Core.Internal;
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
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{

    public class NodeTestFixture : IDisposable
    {
        public ITestOutputHelper Output { get; }

        public  IReadOnlyCollection<IDomainConfiguration> DomainConfigurations => _domainConfigurations;
        private readonly List<IDomainConfiguration> _domainConfigurations = new List<IDomainConfiguration>();

        public NodeTestFixture(ITestOutputHelper output, IDomainConfiguration domainConfiguration) : this(output, domainConfiguration:new[] {domainConfiguration}) { }

        public NodeTestFixture(ITestOutputHelper output,
                               NodeConfiguration cfg=null,
                               IDomainConfiguration[] domainConfiguration = null,
                               TimeSpan? defaultTimeout = null)
        {
            Output = output;
            DefaultTimeout = defaultTimeout ?? DefaultTimeout;
            NodeConfig = cfg ?? new AutoTestNodeConfiguration();
            LoggerConfiguration = new LoggerConfiguration();
            ActorSystemConfigBuilder = new ActorSystemConfigBuilder();

            NodeConfig.ConfigureStandAloneInMemorySystem(ActorSystemConfigBuilder, true);

            TestNodeBuilder = (node, kit) => new TestLocalNode(node, kit);
            NodeBuilder = new GridNodeBuilder()
                          .Timeout(DefaultTimeout)
                          .Transport(sys => sys.InitLocalTransportExtension());

            if (domainConfiguration != null)
                _domainConfigurations.AddRange(domainConfiguration);
        }

        public IActorSystemConfigBuilder ActorSystemConfigBuilder { get; set; }
        public LoggerConfiguration LoggerConfiguration { get; set; }
        public IGridNodeBuilder NodeBuilder { get; set; }
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
            _domainConfigurations.Add(config);
            return this;
        }

        private async Task<IExtendedGridDomainNode> StartNode(IExtendedGridDomainNode node)
        {
            Node = node;
            OnNodeCreatedEvent.Invoke(this, node);
            await Node.Start();
            OnNodeStartedEvent.Invoke(this, Node);

            return Node;
        }

        public Task<IExtendedGridDomainNode> CreateNode(ILogger logger = null)
        {
            return CreateNode(() => NodeConfig.CreateInMemorySystem(), logger);
        }

        public Task<IExtendedGridDomainNode> CreateNode(Func<ActorSystem> actorSystemProvider, ILogger logger = null)
        {
            var log = logger ?? CreateLogger();

            NodeBuilder.DomainConfigurations(DomainConfigurations.ToArray())
                       .Log(log)
                       .Initialize(sys => sys.AttachSerilogLogging(log))
                       .ActorSystem(actorSystemProvider);

            OnNodePreparingEvent.Invoke(this, this);

            var node = NodeBuilder.Build();

            var gridDomainNode = (GridDomainNode) node;

            return StartNode(gridDomainNode);
        }

        private Logger CreateLogger()
        {
            return LoggerConfiguration.Default(NodeConfig.LogLevel)
                                      .XUnit(NodeConfig.LogLevel, Output)
                                      .WriteToFile(NodeConfig.LogLevel,NodeConfig.Name)
                                      .CreateLogger();
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