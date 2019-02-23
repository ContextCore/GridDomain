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
        private static Config _config = new ActorSystemConfigBuilder().Add(LogConfig.All).Build();

        public AggregateActorTests(ITestOutputHelper helper) : base(_config, "aggregateTests",helper)
        {
            var container = new ContainerBuilder();
            container.RegisterInstance<IAggregateDependencies<Cat>>(new AggregateDependencies<Cat>());
            var c = container.Build();
            Sys.InitAggregatesExtension(c);
        }

        [Fact]
        public void AA_can_execute_commands()
        {
            var actor = Sys.ActorOf(Props.Create<AggregateActor<Cat>>(), "myCat");
            actor.Tell(new AggregateActor.ExecuteCommand(new Cat.GetNewCatCommand("myCat"), MessageMetadata.Empty));
            var executed = ExpectMsg<AggregateActor.CommandExecuted>();
        }

        [Fact]
        public async Task AA_can_execute_commands_and_persist_events()
        {
            var actor = Sys.ActorOf(Props.Create<AggregateActor<Cat>>(), "myCat");
            actor.Tell(new AggregateActor.ExecuteCommand(new Cat.GetNewCatCommand("myCat"), MessageMetadata.Empty));
            var executed = ExpectMsg<AggregateActor.CommandExecuted>();

            Watch(actor);
            Sys.Stop(actor);
            this.ExpectTerminated(actor);
            
            var testCat = ActorOfAsTestActorRef<AggregateActor<Cat>>("myCat");
            await Task.Delay(1000);
            Assert.Equal("myCat", testCat.UnderlyingActor.Aggregate.Name);
        }

        [Fact]
        public void AA_can_propagate_commands_exceptions_back()
        {
            var actor = Sys.ActorOf(Props.Create<AggregateActor<Cat>>(), "myCat");
            actor.Tell(new AggregateActor.ExecuteCommand(new Cat.GetNewCatCommand("myCat"), MessageMetadata.Empty));
            var executed = ExpectMsg<AggregateActor.CommandExecuted>();
            actor.Tell(new AggregateActor.ExecuteCommand(new Cat.PetCommand("myCat"), MessageMetadata.Empty));
            var error = ExpectMsg<AggregateActor.CommandFailed>();
        }
    }
}