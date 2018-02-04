using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.RecycleMonitor;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Transport.Extension;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Aggregate_actor_lifetime {


    public class RecyclingMonitorActorTests:TestKit
    {
        public RecyclingMonitorActorTests(ITestOutputHelper output):base("test",output)
        {
            
        }

        [Fact]
        public void Given_monitor_When_time_pass_Then_it_sents_shutdown_request_to_Suspect_and_terminates()
        {
            var recycleConfiguration = new RecycleConfiguration(TimeSpan.FromMilliseconds(100),TimeSpan.FromMilliseconds(50));

            var watched = CreateTestProbe();
            
            var monitor = Sys.ActorOf(Props.Create(() => new RecycleMonitorActor(recycleConfiguration, watched)),"monitor");
            Watch(monitor);
            watched.ExpectMsg<GracefullShutdownRequest>();
            Sys.Stop(watched);
            ExpectTerminated(monitor);
        }
        
        [Fact]
        public void Given_monitor_When_suspect_is_active_Then_shutdown_request_is_delayed()
        {
            var recycleConfiguration = new RecycleConfiguration(TimeSpan.FromMilliseconds(100),TimeSpan.FromMilliseconds(50));

            var watched = CreateTestProbe();
            var monitor = Sys.ActorOf(Props.Create(() => new RecycleMonitorActor(recycleConfiguration, watched)),"monitor");
            Watch(monitor);
            
            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            ExpectNoMsg(TimeSpan.FromMilliseconds(50));
            
            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            ExpectNoMsg(TimeSpan.FromMilliseconds(50));

            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            ExpectNoMsg(TimeSpan.FromMilliseconds(50));

            
            watched.ExpectMsg<GracefullShutdownRequest>();
            Sys.Stop(watched);

            ExpectTerminated(monitor); 
        }
        
        [Fact]
        public void Given_monitor_sent_shutdown_request_When_suspect_is_active_Then_new_shutdown_request_is_scheduled()
        {
            var recycleConfiguration = new RecycleConfiguration(TimeSpan.FromMilliseconds(100),TimeSpan.FromMilliseconds(50));

            var watched = CreateTestProbe();
            
            var monitor = Sys.ActorOf(Props.Create(() => new RecycleMonitorActor(recycleConfiguration, watched)),"monitor");
            Watch(monitor);
            
            watched.ExpectMsg<GracefullShutdownRequest>();
            
            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            watched.ExpectNoMsg(TimeSpan.FromMilliseconds(50));
            
            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            watched.ExpectNoMsg(TimeSpan.FromMilliseconds(50));

            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            watched.ExpectNoMsg(TimeSpan.FromMilliseconds(50));

            watched.ExpectMsg<GracefullShutdownRequest>();
            Sys.Stop(watched);
            ExpectTerminated(monitor); 
        }
        
        
    }
    
    
    public static class AggregateActorDebugExtensions
    {
        public static void TellWithMetadata(this IActorRef actor, object message, IMessageMetadata metadata = null, IActorRef sender=null)
        {
            actor.Tell(new MessageMetadataEnvelop(message,metadata),sender);
        }
    }
    
    public class DoWorkCommand : Command<ShutdownTestAggregate>
    {
        public string Parameter { get; }
        public TimeSpan? Duration { get; }

        public DoWorkCommand(Guid aggregateId, string parameter, TimeSpan? duration) : base(aggregateId)
        {
            Parameter = parameter;
            Duration = duration;
        }
    }

    public class WorkDone : DomainEvent
    {
        public string Value { get; }

        public WorkDone(Guid sourceId, string value):base(sourceId)
        {
            Value = value;
        }    
    }
        
    public class ShutdownTestAggregate : ConventionAggregate
    {
        public static List<string> ExecutedCommands = new List<string>();
            
        public ShutdownTestAggregate(Guid id):base(id)
        {
            Execute<DoWorkCommand>(async c =>
                                   {
                                       if (c.Duration.HasValue)
                                           await Task.Delay(c.Duration.Value);
                                           
                                       Produce(new WorkDone(Id, c.Parameter));
                                   });
            Apply<WorkDone>(e => ExecutedCommands.Add(e.Value));
        }
    }
    
    public class AggregateActorGracefullShutdownTest : TestKit
    {

        public AggregateActorGracefullShutdownTest(ITestOutputHelper output):base("test",output)
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
            
        }  
        
        [Fact]
        public void Given_filled_inbox_and_activity_When_shutdown_Then_terminate_after_activity_and_messages()
        {
            
        }
        
        [Fact]
        public void Given_pending_shutdown_request_When_received_new_message_to_proess_Then_terminate_after_message_process()
        {
            
        }
    }
    
    public class ProcessActorGracefullShutdownTest : TestKit
    {

        [Fact]
        public void Given_empty_inbox_and_no_activity_When_shutdown_Then_terminate_immediatly()
        {
        
        }  
        
        [Fact]
        public void Given_empty_inbox_and_waiting_for_projection_When_shutdown_Then_terminate_after_activity_finish()
        {
            
        }   
        
        [Fact]
        public void Given_empty_inbox_and_waiting_for_persistence_When_shutdown_Then_terminate_after_activity_finish()
        {
            
        }  
        
        [Fact]
        public void Given_filled_inbox_and_no_activity_When_shutdown_Then_terminate_after_message_process()
        {
            
        }  
        
        [Fact]
        public void Given_filled_inbox_and_activity_When_shutdown_Then_terminate_after_activity_and_messages()
        {
            
        }
        
        [Fact]
        public void Given_pending_shutdown_request_When_received_new_message_to_proess_Then_terminate_after_message_process()
        {
            
        }
    }
}