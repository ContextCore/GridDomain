using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using GridDomain.Common;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.CommandPipe;
using GridDomain.Node.Cluster.Configuration;
using GridDomain.Node.Configuration;
using Serilog.Core;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.ClusterConf
{
    /// <summary>
    /// GIVEN actor system builder
    /// WHEN building a cluster
    /// </summary>
    public class ClusterConfigurationTests
    {
        private readonly Logger _logger;

        public ClusterConfigurationTests(ITestOutputHelper output)
        {
            _logger = new XUnitAutoTestLoggerConfiguration(output,
                                                           LogEventLevel.Verbose,
                                                           GetType()
                                                               .Name).CreateLogger();
        }

        [Fact]
        public async Task Cluster_can_start_with_predefined_and_automatic_seed_nodes()
        {
            using (var akkaCluster = await ActorSystemConfigBuilder.New(_logger)
                                                             .Cluster()
                                                             .AutoSeeds(3)
                                                             .Seeds(10010)
                                                             .Workers(1)
                                                             .Build()
                                                             .CreateInTime())
            {
                await CheckClusterStarted(akkaCluster);
            }
        }

        [Fact]
        public async Task Cluster_can_start_with_automatic_seed_nodes()
        {
            using (var akkaCluster = await ActorSystemConfigBuilder.New(_logger)
                                                             .Cluster()
                                                             .AutoSeeds(3)
                                                             .Workers(1)
                                                             .Build()
                                                             .CreateInTime())
            {
                await CheckClusterStarted(akkaCluster);
            }
        }

        [Fact]
        public async Task Cluster_can_start_with_static_seed_nodes()
        {
            using (var akkaCluster = await ActorSystemConfigBuilder.New(_logger)
                                                             .Cluster()
                                                             .Seeds(10011)
                                                             .Workers(1)
                                                             .Build()
                                                             .CreateInTime())
            {
                await CheckClusterStarted(akkaCluster);
            }
        }

        private async Task CheckClusterStarted(ClusterInfo akkaCluster)
        {
            var diagnoseActor = akkaCluster.Cluster.System.ActorOf(Props.Create(() => new SimpleClusterListener()));

            var knownClusterMembers = await diagnoseActor.Ask<MembersExplored>(new GetResult(akkaCluster.Members.Count))
                                                         .TimeoutAfter(TimeSpan.FromSeconds(5000));

            var knownClusterAddresses = knownClusterMembers.Members.Select(m => m.Address)
                                                           .ToArray();

            //All members of cluster should be reachable
            Assert.Equal(akkaCluster.Members,knownClusterAddresses);
        }

        [Fact]
        public async Task Cluster_can_host_an_actor_with_shard_region_with_predefined_seeds()
        {
            using (var akkaCluster = await ActorSystemConfigBuilder.New(_logger)
                                                             .Log(LogEventLevel.Verbose)
                                                             .Cluster()
                                                             .Seeds(10030)
                                                             .AutoSeeds(1)
                                                             .Workers(1)
                                                             .Build()
                                                             .OnClusterUp(ActivateShardRegion)
                                                             .CreateInTime())
            {
                await CheckClusterCanHostAnActor(akkaCluster);
            }
        }

        [Fact]
        public async Task Cluster_can_host_an_actor_with_shard_region_with_auto_seeds()
        {
            using (var akkaCluster = await ActorSystemConfigBuilder.New(_logger)
                                                             .Log(LogEventLevel.Verbose)
                                                             .Cluster()
                                                             .AutoSeeds(2)
                                                             .Workers(2)
                                                             .Build()
                                                             .OnClusterUp(ActivateShardRegion)
                                                             .CreateInTime())
            {
                await CheckClusterCanHostAnActor(akkaCluster);
            }
        }

        private async Task CheckClusterCanHostAnActor(ClusterInfo akkaCluster)
        {
            var actorSystem = akkaCluster.Cluster.System;
            var region = ClusterSharding.Get(actorSystem)
                                        .ShardRegion("my-actor");

            var message = "hello";

            var response = await region.Ask<object>(new ShardEnvelope("1", "1", message, MessageMetadata.Empty),
                                                    TimeSpan.FromSeconds(5));

            Assert.Equal(message, response.ToString());
        }

        private static async Task<IActorRef> ActivateShardRegion(ActorSystem actorSystem)
        {
            string regionName = "my-actor";
            var clusterSharding = ClusterSharding.Get(actorSystem);
            return await clusterSharding.StartAsync(
                                                    regionName,
                                                    Props.Create<EchoShardedActor>(),
                                                    ClusterShardingSettings.Create(actorSystem),
                                                    new MessageExtractor());
        }
    }
}