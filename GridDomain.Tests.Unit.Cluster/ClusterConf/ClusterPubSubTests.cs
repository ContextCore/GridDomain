using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.Configuration;
using GridDomain.Node.Cluster.Transport;
using GridDomain.Node.Configuration;
using Serilog.Core;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.ClusterConf {
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
            using (var akkaCluster = await new ActorSystemConfigBuilder()
                                                             .EmitLogLevel(LogEventLevel.Verbose)
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
}