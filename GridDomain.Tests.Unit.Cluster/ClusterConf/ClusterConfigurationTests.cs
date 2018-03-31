using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Akka.Streams.Implementation.Fusing;
using Akka.TestKit.Xunit2;
using Akka.Util.Internal;
using FluentAssertions;
using GridDomain.Common;
using GridDomain.Node.Cluster;
using GridDomain.Node.Configuration;
using GridDomain.Transport.Extension;
using Serilog.Core;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster
{
    public class ClusterPubSubTests
    {
        private readonly Logger _logger;

        public ClusterPubSubTests(ITestOutputHelper output)
        {
            _logger = new XUnitAutoTestLoggerConfiguration(output,
                                                           LogEventLevel.Verbose,
                                                           GetType()
                                                               .Name).CreateLogger();
        }

        [Fact]
        public async Task Cluster_can_host_a_distributed_pubsub()
        {
            using (var akkaCluster = await ActorSystemBuilder.New(_logger)
                                                             .Log(LogEventLevel.Verbose)
                                                             .Cluster("testNexta")
                                                             .Seeds(10030)
                                                             .AutoSeeds(1)
                                                             .Workers(1)
                                                             .Build()
                                                             .OnClusterUp(StartMessageProducer)
                                                             .CreateInTime())
            {
                var transport = akkaCluster.Cluster.System.InitDistributedTransport().Transport;
                var inbox = Inbox.Create(akkaCluster.Cluster.System);

                await transport.Subscribe<HeartBeatMessage>(inbox.Receiver);
                var addressesToWait = akkaCluster.Members.ToList();
                
                await WaitForAddressesConfirmation(addressesToWait, inbox);
                 //   .TimeoutAfter(TimeSpan.FromSeconds(10));
            }
        }

        private async Task WaitForAddressesConfirmation(List<Address> addressesToWait, Inbox inbox)
        {
            while (addressesToWait.Any())
            {
                var res = await inbox.ReceiveAsync(TimeSpan.FromSeconds(1));
                _logger.Information("Received {res} from pubSub",res);
                if(res is HeartBeatMessage beat)
                    addressesToWait.Remove(beat.Address);
            }
        }

        class HeartBeatMessage
        {
            public string Text { get; }
            public Address Address { get; }

            public HeartBeatMessage(string text, Address address)
            {
                Text = text;
                Address = address;
            }
        }

        class MessageProducerActor : ReceiveActor
        {
            public MessageProducerActor()
            {
                var transport = Context.System.InitDistributedTransport()
                                       .Transport;
                var address = Context.System.GetAddress();
                var msg = new HeartBeatMessage("Hello from " + address, address);
                Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), Self, msg, Self);
                Receive<HeartBeatMessage>(s => transport.Publish(s));
            }
        }

        private Task StartMessageProducer(ActorSystem arg)
        {
            var producerACtor = arg.ActorOf<MessageProducerActor>();
            return Task.CompletedTask;
        }
    }

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
            using (var akkaCluster = await ActorSystemBuilder.New(_logger)
                                                             .Cluster("testPredefined")
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
            using (var akkaCluster = await ActorSystemBuilder.New(_logger)
                                                             .Cluster("testAutoSeed")
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
            using (var akkaCluster = await ActorSystemBuilder.New(_logger)
                                                             .Cluster("testSeed")
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

            await Task.Delay(1000);
            var knownClusterMembers = SimpleClusterListener.KnownMemberList;
            var knownClusterAddresses = knownClusterMembers.Select(m => m.Address)
                                                           .ToArray();

            //All members of cluster should be reachable
            knownClusterAddresses.Should()
                                 .BeEquivalentTo(akkaCluster.Members);
        }

        [Fact]
        public async Task Cluster_can_host_an_actor_with_shard_region_with_predefined_seeds()
        {
            using (var akkaCluster = await ActorSystemBuilder.New(_logger)
                                                             .Log(LogEventLevel.Verbose)
                                                             // .DomainSerialization()
                                                             .Cluster("testNexta")
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
            using (var akkaCluster = await ActorSystemBuilder.New(_logger)
                                                             .Log(LogEventLevel.Verbose)
                                                             .Cluster("testABC")
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