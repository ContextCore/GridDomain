using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AggregateShardingTests
    {
        private readonly Logger _logger;
        private readonly ITestOutputHelper _testOutputHelper;

        public AggregateShardingTests(ITestOutputHelper output)
        {
            _testOutputHelper = output;
            _logger = new XUnitAutoTestLoggerConfiguration(output,
                                                           LogEventLevel.Information,
                                                           GetType()
                                                               .Name).CreateLogger();
        }

        [Fact]
        public async Task Cluster_Node_can_execute_commands_and_wait_for_events()
        {
            var domainFixture = new BalloonFixture(_testOutputHelper);
            List<IGridDomainNode> nodes = new List<IGridDomainNode>();
            using (var cluster = await CreateClusterNodes(domainFixture, nodes))
            {
                var node = nodes.First();

                var res = await node
                                .Prepare(new InflateNewBallonCommand(123, "myBalloon"))
                                .Expect<BalloonCreated>()
                                .Execute();

                Assert.Equal("myBalloon", res.Received.SourceId);
            }
        }

        [Fact]
        public async Task Cluster_Node_can_execute_commands()
        {
            var domainFixture = new BalloonFixture(_testOutputHelper);
            List<IGridDomainNode> nodes = new List<IGridDomainNode>();
            using (var cluster = await CreateClusterNodes(domainFixture, nodes))
            {
                var gridDomainNode = nodes.First();

                await gridDomainNode.Execute(new InflateNewBallonCommand(123, "myBalloon"), CommandConfirmationMode.Executed);
            }
        }

        private async Task<ClusterInfo> CreateClusterNodes(BalloonFixture domainFixture, List<IGridDomainNode> nodes)
        {
            return await ActorSystemBuilder.New(_logger)
                                           .Log(LogEventLevel.Information)
                                           .DomainSerialization()
                                           .Cluster("test")
                                           .AutoSeeds(1)
                                           .Workers(2)
                                           .Build()
                                           .OnClusterUp(async s =>
                                                        {
                                                            var node = await domainFixture.CreateClusterNode(() => s, _logger);
                                                            nodes.Add(node);
                                                        })
                                           .CreateInTime();
        }
    }
}