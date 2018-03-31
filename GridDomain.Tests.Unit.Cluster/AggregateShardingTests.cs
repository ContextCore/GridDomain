using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.DI.Core;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Cluster;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Serilog.Core;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster
{
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
                                                   .CreateInTime();

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
                                                   .CreateInTime();

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