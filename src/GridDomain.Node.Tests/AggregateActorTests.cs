using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.Xunit2;
using Autofac;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;
using GridDomain.Common;
using GridDomain.Domains;
using GridDomain.Node.Akka;
using GridDomain.Node.Akka.Actors.Aggregates;
using GridDomain.Node.Akka.Configuration.Hocon;
using GridDomain.Node.Akka.Extensions.Aggregates;
using GridDomain.Node.Tests.TestJournals.Hocon;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Node.Tests
{
    public class AggregateActorTests : TestKit
    {
        private static readonly Config _config = new ActorSystemConfigBuilder().Add(LogConfig.All()).Add(new TestJournalConfig()).Build();
        private readonly AggregateConfiguration<Cat> _aggregateConfiguration = new AggregateConfiguration<Cat>();

        public AggregateActorTests(ITestOutputHelper helper) : base(_config, "aggregateTests",helper)
        {
            var container = new ContainerBuilder();
            container.RegisterInstance<ICommandsResultAdapter>(new CatCommandsResultAdapter())
                     .Named<ICommandsResultAdapter>(typeof(Cat).BeautyName());
            
            container.RegisterInstance<IAggregateConfiguration<Cat>>(_aggregateConfiguration);
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
        public void AA_use_command_adapter()
        {
            var catAddress = "myCat".AsAddressFor<Cat>();
            var actor = Sys.ActorOf(Props.Create<AggregateActor<Cat>>(), catAddress.ToString());
            actor.Tell(new AggregateActor.ExecuteCommand(new Cat.GetNewCatCommand("myCat"), MessageMetadata.Empty));
            var executed = ExpectMsg<AggregateActor.CommandExecuted>();
            
            Assert.Equal("myCat",executed.Value);
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
            Assert.Equal("myCat", testCat.UnderlyingActor.Aggregate.Id);
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
            Assert.IsType<AggregateActor.CommandExecutionException>(error.Reason);
            Assert.IsType<Cat.IsUnhappyException>(error.Reason.InnerException);
        }
 
        [Fact]
        public void AA_will_not_execute_same_command_twice()
        {
            var catAddress = "myCat".AsAddressFor<Cat>();
            var actor = Sys.ActorOf(Props.Create<AggregateActor<Cat>>(), catAddress.ToString());
            actor.Tell(new AggregateActor.ExecuteCommand(new Cat.GetNewCatCommand("myCat"), MessageMetadata.Empty));
            ExpectMsg<AggregateActor.CommandExecuted>(TimeSpan.FromMinutes(10));
            
            var petCommand = new Cat.FeedCommand("myCat");
            actor.Tell(new AggregateActor.ExecuteCommand(petCommand, MessageMetadata.Empty));
            ExpectMsg<AggregateActor.CommandExecuted>(TimeSpan.FromMinutes(10));

            actor.Tell(new AggregateActor.ExecuteCommand(petCommand, MessageMetadata.Empty));
            var failed =  ExpectMsg<AggregateActor.CommandFailed>(TimeSpan.FromMinutes(10));
            
            Assert.IsType<AggregateActor.CommandExecutionException>(failed.Reason);
            Assert.IsType<CommandAlreadyExecutedException>(failed.Reason.InnerException);
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
            Assert.Equal(actor.Path.ToString(), report.Path);
        }
        
       
        
//        [Fact] //TODO: add a test case 
//        public void AA_will_persist_pending_events_before_shutdown()
//        {
//             throw new NotImplementedException();
//        }
       

    }
}