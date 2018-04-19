using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Cluster.CommandPipe.CommandGrouping;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster
{
    public class CommandGroupTest : TestKit
    {
        public CommandGroupTest(ITestOutputHelper output) : base("")
        {
            Sys.AttachSerilogLogging(new XUnitAutoTestLoggerConfiguration(output,LogEventLevel.Verbose,GetType().Name).CreateLogger());
        }

        class Worker : ReceiveActor
        {
            public static readonly Dictionary<string, int> MessagesReceiveid = new Dictionary<string, int>();
            public int MessagesReceived = 0;

            public Worker()
            {
                var loggingAdapter = Context.GetSeriLogger();

                ReceiveAny(m =>
                           {
                               loggingAdapter.Info("Got a msg {@msg}",m);
                               MessagesReceiveid[Self.Path.Name] = ++MessagesReceived;

                           });
            }
        }

        class Message
        {
            public Message(string desiredGroup)
            {
                DesiredGroup = desiredGroup;
            }

            public string DesiredGroup { get; }
        }

        class TestCommand : Command
        {
            public TestCommand(string desiredGroup) : base("", desiredGroup) { }
        }

        [Fact]
        public async Task CheckGroupDistribution_With_commands()
        {
            var workerA = Sys.ActorOf(Props.Create(() => new Worker()), "A");
            var workerB = Sys.ActorOf(Props.Create(() => new Worker()), "B");
            var workerC = Sys.ActorOf(Props.Create(() => new Worker()), "C");

            this.Log.Info("Created worker on path: " + workerA.Path);
            this.Log.Info("Created worker on path: " + workerB.Path);
            this.Log.Info("Created worker on path: " + workerC.Path);


            var groupROuter = new ConsistentMapGroup(new Dictionary<string, string>
                                               {
                                                   {"A", "user/A"},
                                                   {"B", "user/B"},
                                                   {"C", "user/C"}
                                               });

            var routed = Sys.ActorOf(Props.Empty.WithRouter(groupROuter));


            routed.Tell(new TestCommand("A"));
            routed.Tell(new TestCommand("B"));
            routed.Tell(new TestCommand("C"));
            routed.Tell(new TestCommand("A"));
            routed.Tell(new TestCommand("A"));
            routed.Tell(new TestCommand("B"));
            routed.Tell(new TestCommand("C"));
            routed.Tell(new TestCommand("C"));

            await Task.Delay(1000);
            
            Assert.Equal(3, Worker.MessagesReceiveid["A"]);
            Assert.Equal(2, Worker.MessagesReceiveid["B"]);
            Assert.Equal(3, Worker.MessagesReceiveid["C"]);
        }

        [Fact]
        public async Task CheckGroupDistribution_With_commands_envelops()
        {
            var workerA = Sys.ActorOf(Props.Create(() => new Worker()), "A");
            var workerB = Sys.ActorOf(Props.Create(() => new Worker()), "B");
            var workerC = Sys.ActorOf(Props.Create(() => new Worker()), "C");

            this.Log.Info("Created worker on path: " + workerA.Path);
            this.Log.Info("Created worker on path: " + workerB.Path);
            this.Log.Info("Created worker on path: " + workerC.Path);


            var groupROuter = new ConsistentMapGroup(new Dictionary<string, IActorRef>
                                               {
                                                   {"A",workerA},
                                                   {"B",workerB},
                                                   {"C",workerC}
                                               });


            var routed = Sys.ActorOf(Props.Empty.WithRouter(groupROuter));


            routed.Tell(MessageMetadataEnvelop.New(new TestCommand("A")));
            routed.Tell(MessageMetadataEnvelop.New(new TestCommand("B")));
            routed.Tell(MessageMetadataEnvelop.New(new TestCommand("C")));
            routed.Tell(MessageMetadataEnvelop.New(new TestCommand("A")));
            routed.Tell(MessageMetadataEnvelop.New(new TestCommand("A")));
            routed.Tell(MessageMetadataEnvelop.New(new TestCommand("B")));
            routed.Tell(MessageMetadataEnvelop.New(new TestCommand("C")));
            routed.Tell(MessageMetadataEnvelop.New(new TestCommand("C")));

            await Task.Delay(1000);
            
            Assert.Equal(3, Worker.MessagesReceiveid["A"]);
            Assert.Equal(2, Worker.MessagesReceiveid["B"]);
            Assert.Equal(3, Worker.MessagesReceiveid["C"]);
        }

            [Fact]
            public async Task CheckGroupDistribution_With_CustomMap()
            {
                var workerA = Sys.ActorOf(Props.Create(() => new Worker()), "A");
                var workerB = Sys.ActorOf(Props.Create(() => new Worker()), "B");
                var workerC = Sys.ActorOf(Props.Create(() => new Worker()), "C");
        
                this.Log.Info("Created worker on path: " + workerA.Path);
                this.Log.Info("Created worker on path: " + workerB.Path);
                this.Log.Info("Created worker on path: " + workerC.Path);
        
                var groupROuter = new ConsistentMapGroup(new Dictionary<string, IActorRef>
                                                   {
                                                       {"A",workerA},
                                                       {"B",workerB},
                                                       {"C",workerC}
                                                   }).WithMapping(s => ((Message) s).DesiredGroup);
             
                var routed = Sys.ActorOf(Props.Empty.WithRouter(groupROuter));
        
        
                routed.Tell(new Message("A"));
                routed.Tell(new Message("B"));
                routed.Tell(new Message("C"));
                routed.Tell(new Message("A"));
                routed.Tell(new Message("A"));
                routed.Tell(new Message("B"));
                routed.Tell(new Message("C"));
                routed.Tell(new Message("C"));
        
                await Task.Delay(1000);
                
                Assert.Equal(3, Worker.MessagesReceiveid["A"]);
                Assert.Equal(2, Worker.MessagesReceiveid["B"]);
                Assert.Equal(3, Worker.MessagesReceiveid["C"]);
            }
    }
}