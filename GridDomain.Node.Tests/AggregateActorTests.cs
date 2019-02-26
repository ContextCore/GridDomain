using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.Xunit2;
using Autofac;
using GridDomain.Aggregates;
using GridDomain.Common;
using GridDomain.Node.Akka;
using GridDomain.Node.Akka.Actors.Aggregates;
using GridDomain.Node.Akka.AggregatesExtension;
using GridDomain.Node.Akka.Configuration.Hocon;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Node.Tests
{
    public class AggregateActorTests : TestKit
    {
        private static readonly Config _config = new ActorSystemConfigBuilder().Add(LogConfig.All).Build();
        private readonly AggregateDependencies<Cat> _aggregateDependencies = new AggregateDependencies<Cat>();

        public AggregateActorTests(ITestOutputHelper helper) : base(_config, "aggregateTests",helper)
        {
            var container = new ContainerBuilder();
            container.RegisterInstance<IAggregateDependencies<Cat>>(_aggregateDependencies);
            var c = container.Build();
            Sys.InitAggregatesExtension(c);
        }

        [Fact]
        public void AA_can_execute_commands()
        {
            var catAddress = "myCat".AsAddressFor<Cat>();
            var actor = Sys.ActorOf(Props.Create<AggregateActor<Cat>>(), catAddress.ToString());
            actor.Tell(new AggregateActor.ExecuteCommand(new Cat.GetNewCatCommand("myCat"), MessageMetadata.Empty));
            var executed = ExpectMsg<AggregateActor.CommandExecuted>();
        }

        [Fact]
        public async Task AA_can_execute_commands_and_persist_events()
        {
            var catAddress = "myCat".AsAddressFor<Cat>();
            var actor = Sys.ActorOf(Props.Create<AggregateActor<Cat>>(), catAddress.ToString());
            actor.Tell(new AggregateActor.ExecuteCommand(new Cat.GetNewCatCommand("myCat"), MessageMetadata.Empty));
            var executed = ExpectMsg<AggregateActor.CommandExecuted>(TimeSpan.FromHours(1));

            Watch(actor);
            actor.Tell(AggregateActor.ShutdownGratefully.Instance);
            ExpectTerminated(actor);
            
            var testCat = ActorOfAsTestActorRef<AggregateActor<Cat>>(catAddress.ToString());
            await Task.Delay(1000);
            Assert.Equal("myCat", testCat.UnderlyingActor.Aggregate.Name);
        }

        [Fact]
        public void AA_can_propagate_commands_exceptions_back()
        {
            var catAddress = "myCat".AsAddressFor<Cat>();
            var actor = Sys.ActorOf(Props.Create<AggregateActor<Cat>>(), catAddress.ToString());
            actor.Tell(new AggregateActor.ExecuteCommand(new Cat.GetNewCatCommand("myCat"), MessageMetadata.Empty));
            var executed = ExpectMsg<AggregateActor.CommandExecuted>();
            actor.Tell(new AggregateActor.ExecuteCommand(new Cat.PetCommand("myCat"), MessageMetadata.Empty));
            var error = ExpectMsg<AggregateActor.CommandFailed>();
        }
 
        
        [Fact]
        public void AA_can_shutdown_on_request()
        {
            var catAddress = "myCat".AsAddressFor<Cat>();
            var actor = Sys.ActorOf(Props.Create<AggregateActor<Cat>>(), catAddress.ToString());

            Watch(actor);
            actor.Tell(AggregateActor.ShutdownGratefully.Instance);
            ExpectTerminated(actor);
        }
        
        [Fact]
        public void AA_will_send_healthReport_on_request()
        {
            var catAddress = "myCat".AsAddressFor<Cat>();
            var actor = Sys.ActorOf(Props.Create<AggregateActor<Cat>>(), catAddress.ToString());

            actor.Tell(AggregateActor.CheckHealth.Instance);
            var report = ExpectMsg<AggregateHealthReport>();
            Assert.Equal(actor.Path.ToString(), report.Location);
        }
        
       
        
//        [Fact] //TODO: add a test case 
//        public void AA_will_persist_pending_events_before_shutdown()
//        {
//             throw new NotImplementedException();
//        }
       

    }
}