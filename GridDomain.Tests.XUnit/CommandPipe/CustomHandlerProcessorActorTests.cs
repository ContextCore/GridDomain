using System;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.CommandPipe
{
   
    public class CustomHandlerProcessorActorTests : TestKit
    {
       [Fact]
        public void CustomHandlerProcessor_routes_events_by_type()
        {
            var catalog = new CustomHandlersProcessCatalog();
            catalog.Add<SampleAggregateCreatedEvent>(new Processor(TestActor));
            var actor = Sys.ActorOf(Props.Create(() => new HandlersProcessActor(catalog, TestActor)));

            var msg = new MessageMetadataEnvelop<DomainEvent[]>(new DomainEvent[] { new SampleAggregateCreatedEvent("1", Guid.NewGuid())}, 
                                                                MessageMetadata.Empty);

            actor.Tell(msg);

            //TestActor as processor receives message for work
            ExpectMsg<MessageMetadataEnvelop<DomainEvent>>();
            //HandlersProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<AllHandlersCompleted>();
            //HandlersProcessActor should resend domain event to next step - saga actor - for processing
            ExpectMsg<MessageMetadataEnvelop<DomainEvent[]>>();
        }

   

       [Fact]
        public void All_async_handlers_performs_in_parralel()
        {
            var delayActor = Sys.ActorOf(Props.Create(() => new EchoSleepActor(TimeSpan.FromMilliseconds(100), TestActor)));
            var catalog = new CustomHandlersProcessCatalog();

            catalog.Add<SampleAggregateCreatedEvent>(new Processor(delayActor));
            catalog.Add<SampleAggregateChangedEvent>(new Processor(delayActor));

            var actor = Sys.ActorOf(Props.Create(() => new HandlersProcessActor(catalog, TestActor)));

            var sampleAggregateCreatedEvent = new SampleAggregateCreatedEvent("1", Guid.NewGuid());
            var sampleAggregateChangedEvent = new SampleAggregateChangedEvent("1", Guid.NewGuid());

            var msgA = new MessageMetadataEnvelop<DomainEvent[]>(new DomainEvent[] {
                                                                          sampleAggregateCreatedEvent,
                                                                          sampleAggregateChangedEvent},
                                                                 MessageMetadata.Empty);

            actor.Tell(msgA);

            //HandlersProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<AllHandlersCompleted>();
            //HandlersProcessActor should notify next step - saga actor that work is done
            ExpectMsg<IMessageMetadataEnvelop<DomainEvent[]>>();

            //but handlers will finish their work later in undefined sequence
            ExpectMsg<HandlerExecuted>();
            ExpectMsg<HandlerExecuted>();
        }

       [Fact]
        public void All_sync_handlers_performs_one_after_one()
        {
            var delayActor = Sys.ActorOf(Props.Create(() => new EchoSleepActor(TimeSpan.FromMilliseconds(50), TestActor)));
            var catalog = new CustomHandlersProcessCatalog();

            catalog.Add<SampleAggregateCreatedEvent>(new Processor(delayActor, MessageProcessPolicy.Sync));
            catalog.Add<SampleAggregateChangedEvent>(new Processor(delayActor, MessageProcessPolicy.Sync));
            catalog.Add<SampleAggregateChangedEvent>(new Processor(delayActor, MessageProcessPolicy.Sync));

            var actor = Sys.ActorOf(Props.Create(() => new HandlersProcessActor(catalog, TestActor)));

            var msgA = new MessageMetadataEnvelop<DomainEvent[]>(new [] {(DomainEvent)
                                                                         new SampleAggregateCreatedEvent("1", Guid.NewGuid()),
                                                                         new SampleAggregateChangedEvent("1", Guid.NewGuid())},
                                                                 MessageMetadata.Empty);

            actor.Tell(msgA);

            //in sync process we should wait for handlers execution
            //in same order as they were sent to handlers process actor
            ExpectMsg<HandlerExecuted>(e => e.ProcessingMessage.Message is SampleAggregateCreatedEvent);
            ExpectMsg<HandlerExecuted>(e => e.ProcessingMessage.Message is SampleAggregateChangedEvent);
            ExpectMsg<HandlerExecuted>(e => e.ProcessingMessage.Message is SampleAggregateChangedEvent);

            //HandlersProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<AllHandlersCompleted>();
            //HandlersProcessActor should notify next step - saga actor that work is done
            ExpectMsg<IMessageMetadataEnvelop<DomainEvent[]>>();
        }

        
       [Fact]
        public void Sync_and_async_handlers_performs_independent()
        {
            var delayActor = Sys.ActorOf(Props.Create(() => new EchoSleepActor(TimeSpan.FromMilliseconds(50), TestActor)));
            var catalog = new CustomHandlersProcessCatalog();

            catalog.Add<SampleAggregateCreatedEvent>(new Processor(delayActor, MessageProcessPolicy.Sync));
            catalog.Add<SampleAggregateChangedEvent>(new Processor(delayActor, MessageProcessPolicy.Sync));
            catalog.Add<DomainEvent>(new Processor(TestActor));

            var actor = Sys.ActorOf(Props.Create(() => new HandlersProcessActor(catalog, TestActor)));

            var msgA = new MessageMetadataEnvelop<DomainEvent[]>(new [] { new SampleAggregateCreatedEvent("1", Guid.NewGuid()),
                                                                          new SampleAggregateChangedEvent("1", Guid.NewGuid()),
                                                                          new DomainEvent(Guid.NewGuid())},
                                                                 MessageMetadata.Empty);

            actor.Tell(msgA);

            //async event fires immidiately
            ExpectMsg<IMessageMetadataEnvelop>();
        
            //in sync process we should wait for handlers execution
            //in same order as they were sent to handlers process actor
            ExpectMsg<HandlerExecuted>(e => e.ProcessingMessage.Message is SampleAggregateCreatedEvent);
            ExpectMsg<HandlerExecuted>(e => e.ProcessingMessage.Message is SampleAggregateChangedEvent);

            //HandlersProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<AllHandlersCompleted>();
            //HandlersProcessActor should notify next step - saga actor that work is done
            ExpectMsg<IMessageMetadataEnvelop<DomainEvent[]>>();
        }

       [Fact]
        public void CustomHandlerExecutor_does_not_support_domain_event_inheritance()
        {
            var catalog = new CustomHandlersProcessCatalog();
            catalog.Add<SampleAggregateCreatedEvent>(new Processor(TestActor));
            var actor = Sys.ActorOf(Props.Create(() => new HandlersProcessActor(catalog, TestActor)));

            var msg = new MessageMetadataEnvelop<DomainEvent[]>(new [] { new DomainEvent(Guid.NewGuid())}, MessageMetadata.Empty);

            actor.Tell(msg);

            //processor did not run, but we received processing complete message
            ExpectMsg<AllHandlersCompleted>();
        }
    }
}