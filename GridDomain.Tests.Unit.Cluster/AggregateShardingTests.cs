using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.Node;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Cluster;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Serilog;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster
{
    public static class ClusterConfigExtensions
    {
        public static ClusterInfo CreateCluster(this ClusterConfig cfg, ILogger log)
        {
            return cfg.CreateCluster(s =>
                                     {
                                         s.AttachSerilogLogging(log);
                                         s.InitDistributedTransport();
                                     });
        }
    }

    public static class NodeTestFixtureExtensions
    {
        public static async Task<IGridDomainNode> CreateClusterNode(this NodeTestFixture fxt, Func<Akka.Cluster.Cluster> clusterProducer, ILogger log)
        {
            var node = new GridNodeBuilder().PipeFactory(new DelegateActorSystemFactory(()=> clusterProducer().System))
                                            .DomainConfigurations(fxt.DomainConfigurations.ToArray())
                                            .Log(log)
                                            .Timeout(fxt.DefaultTimeout)
                                            .BuildCluster();

            return await fxt.StartNode((GridDomainNode)node);
        }
    }

    public class AggregateShardingTests:IDisposable
    {
        private ClusterInfo _akkaCluster;
        private readonly Logger _logger;
        private readonly ITestOutputHelper _testOutputHelper;

        public AggregateShardingTests(ITestOutputHelper output)
        {
            _logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }

        [Fact]
        public async Task Cluster_Node_can_execute_commands()
        {
            var domainFixture = new BalloonFixture(_testOutputHelper);

            _akkaCluster = ActorSystemBuilder.New()
                                             .DomainSerialization()
                                             .Cluster("test")
                                             .Seeds(9001)
                                             .Workers(1)
                                             .Build()
                                             .CreateCluster(_logger);

            IGridDomainNode node = await domainFixture.CreateClusterNode(() => _akkaCluster.Cluster, _logger);

            var res = await node.Prepare(new BlowBalloonCommand("myBalloon"))
                                .Expect<BalloonCreated>()
                                .Execute();
            
             Assert.Equal("myBalloon",res.Received.SourceId);
        }
        [Fact]
        public void Cluster_can_start_with_static_seed_nodes()
        {
            _akkaCluster = ActorSystemBuilder.New()
                                             .Cluster("testSeed")
                                             .Seeds(8000)
                                             .Workers(1)
                                             .Build()
                                             .CreateCluster(s => s.AttachSerilogLogging(_logger));
        }

        public void Dispose()
        {
            if (_akkaCluster == null) return;
            CoordinatedShutdown.Get(_akkaCluster?.Cluster.System)?.Run().Wait(TimeSpan.FromSeconds(2));
            _akkaCluster = null;
        }
    }
}