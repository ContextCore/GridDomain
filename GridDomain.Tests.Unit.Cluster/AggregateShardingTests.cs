using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Cluster;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster
{
    public static class ClusterConfigExtensions
    {
        public static Task<ClusterInfo> CreateCluster(this ClusterConfig cfg, ILogger log, TimeSpan? timeout = null)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(15);
            
            return cfg.CreateCluster(s =>
                                     {
                                         s.AttachSerilogLogging(log);
                                         s.InitDistributedTransport();
                                     }).TimeoutAfter(timeout.Value,"Cluster was not formed in time");
        }
    }

    public static class NodeTestFixtureExtensions
    {
        public static async Task<IGridDomainNode> CreateClusterNode(this NodeTestFixture fxt, Func<Akka.Cluster.Cluster> clusterProducer, ILogger log)
        {
            var node = new GridNodeBuilder().PipeFactory(new DelegateActorSystemFactory(() => clusterProducer()
                                                                                            .System))
                                            .DomainConfigurations(fxt.DomainConfigurations.ToArray())
                                            .Log(log)
                                            .Timeout(fxt.DefaultTimeout)
                                            .BuildCluster();

            return await fxt.StartNode((GridDomainNode) node);
        }
    }

    public class AggregateShardingTests : IDisposable
    {
        private ClusterInfo _akkaCluster;
        private readonly Logger _logger;
        private readonly ITestOutputHelper _testOutputHelper;

        public AggregateShardingTests(ITestOutputHelper output)
        {
            _logger = new XUnitAutoTestLoggerConfiguration(output,
                                                           LogEventLevel.Information,
                                                           GetType()
                                                               .Name).CreateLogger();
        }

        [Fact]
        public async Task Cluster_Node_can_execute_commands_and_wait_for_events()
        {
            var domainFixture = new BalloonFixture(_testOutputHelper);

            _akkaCluster = await ActorSystemBuilder.New()
                                                   .DomainSerialization()
                                                   .Cluster("test")
                                                   .AutoSeeds(1)
                                                   .Workers(1)
                                                   .Build()
                                                   .CreateCluster(_logger);

            IGridDomainNode node = await domainFixture.CreateClusterNode(() => _akkaCluster.Cluster, _logger);

            var res = await node.Prepare(new InflateNewBallonCommand(123, "myBalloon"))
                                .Expect<BalloonCreated>()
                                .Execute();

            Assert.Equal("myBalloon", res.Received.SourceId);
        }

        [Fact]
        public async Task Cluster_Node_can_execute_commands()
        {
            var domainFixture = new BalloonFixture(_testOutputHelper);

            _akkaCluster = await ActorSystemBuilder.New()
                                                   // .DomainSerialization()
                                                   .Cluster("test")
                                                   .AutoSeeds(1)
                                                   .Workers(1)
                                                   .Build()
                                                   .CreateCluster(_logger);

            IGridDomainNode node = await domainFixture.CreateClusterNode(() => _akkaCluster.Cluster, _logger);

            await node.Execute(new InflateNewBallonCommand(123, "myBalloon"));
        }

        public void Dispose()
        {
            if (_akkaCluster == null) return;
            CoordinatedShutdown.Get(_akkaCluster?.Cluster.System)
                               ?.Run()
                               .Wait(TimeSpan.FromSeconds(2));
            _akkaCluster = null;
        }
    }
}