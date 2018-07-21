using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Cluster.Sharding;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.Configuration;
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
    public class AggregateShardingTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AggregateShardingTests(ITestOutputHelper output)
        {
            _testOutputHelper = output;
        }

        [Fact]
        public async Task Cluster_Node_can_execute_commands_and_wait_for_events()
        {
            using (var cluster = await CreateClusterNodes("canExecuteCommandsWithEventsWait", nameof(Cluster_Node_can_execute_commands_and_wait_for_events)))
            {
                var node = cluster.Nodes.First();

                var res = await node.Prepare(new InflateNewBallonCommand(123, "myBalloon"))
                                    .Expect<BalloonCreated>()
                                    .Execute();

                Assert.Equal("myBalloon", res.Received.SourceId);
            }
        }

        [Fact]
        public async Task Cluster_Node_can_execute_commands()
        {
            using (var cluster = await CreateClusterNodes("canExecuteCOmmands", nameof(Cluster_Node_can_execute_commands)))
            {
                var gridDomainNode = cluster.Nodes.First();

                await gridDomainNode.Execute(new InflateNewBallonCommand(123, "myBalloon"), CommandConfirmationMode.Executed);
            }
        }

        class ClusterNodes : IDisposable
        {
            public ClusterNodes(IReadOnlyCollection<IGridDomainNode> nodes, ClusterInfo cluster)
            {
                Nodes = nodes;
                Cluster = cluster;
            }

            public ClusterInfo Cluster { get; }
            public IReadOnlyCollection<IGridDomainNode> Nodes { get; }

            public void Dispose()
            {
                Cluster?.Dispose();
            }
        }

        private async Task<ClusterNodes> CreateClusterNodes(string clusterName, string logFileName)
        {
            var nodes = new List<IGridDomainNode>();

            var clusterInfo = await new ActorSystemConfigBuilder()
                                    .Log(LogEventLevel.Verbose)
                                    .DomainSerialization()
                                    .Cluster(clusterName)
                                    .AutoSeeds(2)
                                    .Workers(2)
                                    .Build()
                                    .OnClusterUp(async s =>
                                                 {
                                                     try
                                                     {
                                                         var fixture = new BalloonFixture(_testOutputHelper).LogToFile(logFileName);
                                                         var node = await fixture.CreateNode(() => s);
                                                         nodes.Add(node);
                                                     }
                                                     catch (Exception ex)
                                                     {
                                                         var errorLogger = new LoggerConfiguration().WriteToFile(LogEventLevel.Verbose, logFileName)
                                                                                                    .CreateLogger();
                                                         errorLogger.Error(ex, "Error during grid node creation for cluster");
                                                     }
                                                 })
                                    .CreateInTime();

            return new ClusterNodes(nodes, clusterInfo);
        }
    }
}