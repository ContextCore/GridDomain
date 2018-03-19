using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Cluster;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Unit.BalloonDomain;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster
{
    public class AggregatesSharding
    {
        private ClusterInfo _akkaCluster;
        private readonly Logger _logger;
        private readonly ITestOutputHelper _testOutputHelper;

        public AggregatesSharding(ITestOutputHelper output)
        {
            _testOutputHelper = output;
        }

        [Fact]
        public void TestRandom()
        {
           // var random = new Random(123);
           // _testOutputHelper.WriteLine(random.Next(10).ToString());
           // _testOutputHelper.WriteLine(random.Next(10).ToString());
           // _testOutputHelper.WriteLine(random.Next(10).ToString());
            
        }
        [Fact]
        public async Task Cluster_can_host_an_actor_with_shard_region_with_auto_seeds()
        {
            var domainFixture = new BalloonFixture(_testOutputHelper);
            
            
            _akkaCluster = ActorSystemBuilder.New()
                                             .DomainSerialization()
                                             .Cluster("test")
                                             .AutoSeeds(2)
                                             .Workers(2)
                                             .Build().CreateCluster(s => s.AttachSerilogLogging(_logger));
            var system = _akkaCluster.Cluster.System;
            
            var node = domainFixture.CreateNode(() =>system, _logger);

            var props = system.DI()
                              .Props<AggregateActor<Balloon>>();
            
            //var region = await ClusterSharding.Get(system)
            //                                  .StartAsync(
            //                                              typeName: "balloonAggregate",
            //                                              entityProps: props,
            //                                              settings: ClusterShardingSettings.Create(actorSystem),
            //                                              messageExtractor: new MessageExtractor());
// send message to entity through shard region
// send message to entity through shard region
            //var message = "hello";
            //var response = await region.Ask<object>(new ShardEnvelope("1", "1", message, MessageMetadata.Empty),
            //                                        TimeSpan.FromSeconds(5));
            //
            //Assert.Equal(message.ToString(), response.ToString());
            // Dispose();
        }
    }
}