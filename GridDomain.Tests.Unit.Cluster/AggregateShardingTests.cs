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
            return cfg.CreateCluster(s => s.AttachSerilogLogging(log));
        }
    }


    public static class NodeTestFixtureExtensions
    {
        public static IGridDomainNode CreateClusterNode(this NodeTestFixture fxt, Func<Akka.Cluster.Cluster> clusterProducer, ILogger log)
        {
            return null;
        }
    }
    
    public class AggregateShardingTests
    {
        private ClusterInfo _akkaCluster;
        private readonly Logger _logger;
        private readonly ITestOutputHelper _testOutputHelper;

        public AggregateShardingTests(ITestOutputHelper output)
        {
            _testOutputHelper = output;
        }

        [Fact]
        public async Task Cluster_Node_can_execute_commands()
        {
            var domainFixture = new BalloonFixture(_testOutputHelper);


            _akkaCluster = ActorSystemBuilder.New()
                                             .DomainSerialization()
                                             .Cluster("test")
                                             .Seeds(10100)
                                             .AutoSeeds(2)
                                             .Workers(2)
                                             .Build()
                                             .CreateCluster(_logger);


            IGridDomainNode node = domainFixture.CreateClusterNode(() => _akkaCluster.Cluster, _logger);

            var res = await node.Prepare(new BlowBalloonCommand("myBalloon"))
                                .Expect<BalloonCreated>()
                                .Execute();
            
             Assert.Equal("myBalloon",res.Received.SourceId);
        }
    }
}