using System;
using System.Linq;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using GridDomain.Transport.Extension;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.AggregateLifetime.GracefulShutdown {
    public class AggregateActorGracefullShutdownTest : TestKit
    {

        public AggregateActorGracefullShutdownTest(ITestOutputHelper output):base(new ActorSystemBuilder().Log(LogEventLevel.Debug).BuildHocon(),"test",output)
        {
            Sys.InitLocalTransportExtension();
        }

        
        [Fact]
        public void Given_empty_inbox_and_no_activity_When_shutdown_Then_terminate_immediatly()
        {
            var handlersActor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(new HandlersDefaultProcessor(), TestActor)));
            var actor = CreateAggregateActor(handlersActor, Guid.NewGuid());
            Watch(actor);
            actor.Tell(GracefullShutdownRequest.Instance);
            
            ExpectTerminated(actor);
            
            Assert.Empty(ShutdownTestAggregate.ExecutedCommands);
        }

        private IActorRef CreateAggregateActor(IActorRef handlersActor, Guid actorId)
        {
            var actor = Sys.ActorOf(Props.Create(() => new AggregateActor<ShutdownTestAggregate>(CommandAggregateHandler.New<ShutdownTestAggregate>(null),
                                                                                                new SnapshotsPersistencePolicy(1, 10, null, null),
                                                                                                AggregateFactory.Default,
                                                                                                AggregateFactory.Default,
                                                                                                handlersActor)),
                                    EntityActorName.New<ShutdownTestAggregate>(actorId).Name);
            return actor;
        }

        [Fact]
        public void Given_empty_inbox_and_activity_When_shutdown_Then_terminate_after_activity_finish()
        {
            var handlersActor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(new HandlersDefaultProcessor(), TestActor)));
            var aggregateId = Guid.NewGuid();
            var actor = CreateAggregateActor(handlersActor, aggregateId);
            
            Watch(actor);
            
            actor.TellWithMetadata(new DoWorkCommand(aggregateId,"1",TimeSpan.FromSeconds(1)));
            actor.Tell(GracefullShutdownRequest.Instance);
            this.AwaitAssert(() => Assert.Equal("1",ShutdownTestAggregate.ExecutedCommands.First()),
                                                TimeSpan.FromSeconds(2),
                                                TimeSpan.FromMilliseconds(50));
            ExpectTerminated(actor);
        }  
        
        [Fact]
        public void Given_filled_inbox_and_no_activity_When_shutdown_Then_terminate_after_message_process()
        {
                throw new NotImplementedException();
        }  
        
        [Fact]
        public void Given_filled_inbox_and_activity_When_shutdown_Then_terminate_after_activity_and_messages()
        {
            throw new NotImplementedException();

        }
        
        [Fact]
        public void Given_pending_shutdown_request_When_received_new_message_to_proess_Then_terminate_after_message_process()
        {
            throw new NotImplementedException();

        }
    }
    
}