using System;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.CommandPipe.Processors;
using GridDomain.Node.Actors.Hadlers;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;

namespace GridDomain.Tests.Unit.CommandPipe
{
    public class CustomHandlerProcessorActorTests : TestKit
    {
        [Fact]
        public void All_async_handlers_performs_in_parralel()
        {
            var delayActor = Sys.ActorOf(Props.Create(() => new EchoSleepActor(TimeSpan.FromMilliseconds(100), TestActor)));
            var catalog = new ProcessorListCatalog();

            catalog.Add<BalloonCreated>(new FireAndForgetMessageProcessor(delayActor));
            catalog.Add<BalloonTitleChanged>(new FireAndForgetMessageProcessor(delayActor));

            var actor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(catalog, TestActor)));

            var sampleAggregateCreatedEvent = new BalloonCreated("1", Guid.NewGuid());
            var sampleAggregateChangedEvent = new BalloonTitleChanged("1", Guid.NewGuid());

            var msgA = new MessageMetadataEnvelop<Project>(new Project(sampleAggregateCreatedEvent, sampleAggregateChangedEvent),
                                                           MessageMetadata.Empty);

            actor.Tell(msgA);

            //HandlersProcessActor should notify next step - saga actor that work is done
            ExpectMsg<IMessageMetadataEnvelop<DomainEvent>>();
            ExpectMsg<IMessageMetadataEnvelop<DomainEvent>>();

            //HandlersProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<AllHandlersCompleted>();

            //but handlers will finish their work later in undefined sequence
            ExpectMsg<HandlerExecuted>();
            ExpectMsg<HandlerExecuted>();
        }


        [Fact]
        public void Given_no_processors_pipe_still_reply_with_completed_messages()
        {
            var catalog = new ProcessorListCatalog();
            var actor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(catalog, TestActor)));

            var sampleAggregateCreatedEvent = new BalloonCreated("1", Guid.NewGuid());

            var msgA = new MessageMetadataEnvelop<Project>(new Project(sampleAggregateCreatedEvent),
                                                           MessageMetadata.Empty);

            actor.Tell(msgA);

            //HandlersProcessActor should notify next step - saga actor that work is done
            ExpectMsg<IMessageMetadataEnvelop<DomainEvent>>();
            //HandlersProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<AllHandlersCompleted>();
        }

        [Fact]
        public void All_sync_handlers_performs_one_after_one()
        {
            var delayActor = Sys.ActorOf(Props.Create(() => new EchoSleepActor(TimeSpan.FromMilliseconds(50), TestActor)));
            var catalog = new ProcessorListCatalog();

            catalog.Add<BalloonCreated>(new SyncProjectionProcessor(delayActor));
            catalog.Add<BalloonTitleChanged>(new SyncProjectionProcessor(delayActor));
            catalog.Add<BalloonTitleChanged>(new SyncProjectionProcessor(delayActor));

            var actor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(catalog, TestActor)));

            var msgA =
                new MessageMetadataEnvelop<Project>(new Project(new BalloonCreated("1", Guid.NewGuid()),
                                                                new BalloonTitleChanged("1", Guid.NewGuid())),
                                                    MessageMetadata.Empty);

            actor.Tell(msgA);

            //in sync process we should wait for handlers execution
            //in same order as they were sent to handlers process actor
            ExpectMsg<HandlerExecuted>(e => e.ProcessingMessage.Message is BalloonCreated);
            ExpectMsg<HandlerExecuted>(e => e.ProcessingMessage.Message is BalloonTitleChanged);
            ExpectMsg<HandlerExecuted>(e => e.ProcessingMessage.Message is BalloonTitleChanged);

            //HandlersProcessActor should notify next step - saga actor that work is done
            ExpectMsg<IMessageMetadataEnvelop<DomainEvent>>();
            ExpectMsg<IMessageMetadataEnvelop<DomainEvent>>();

            //HandlersProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<AllHandlersCompleted>();
        }

        class Inherited : BalloonCreated
        {
            public Inherited() : base("inherited", Guid.NewGuid()) { }
        }

        [Fact]
        public void CustomHandlerExecutor_does_not_support_domain_event_inheritance()
        {
            var catalog = new ProcessorListCatalog();
            catalog.Add<BalloonCreated>(new SyncProjectionProcessor(TestActor));
            var actor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(catalog, TestActor)));

            var msg = MessageMetadataEnvelop.New(new Project(new Inherited()));

            actor.Tell(msg);
            //processor did not run, but we pass message to sagas after
            ExpectMsg<MessageMetadataEnvelop<DomainEvent>>();
            //processor did not run, but we received processing complete message
            ExpectMsg<AllHandlersCompleted>();
        }

        [Fact]
        public void CustomHandlerProcessor_routes_events_by_type()
        {
            var catalog = new ProcessorListCatalog();
            catalog.Add<BalloonCreated>(new FireAndForgetMessageProcessor(TestActor));
            var actor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(catalog, TestActor)));

            var msg = new MessageMetadataEnvelop<Project>(new Project(new BalloonCreated("1", Guid.NewGuid())),
                                                          MessageMetadata.Empty);

            actor.Tell(msg);
           
            //TestActor as processor receives message for work
            ExpectMsg<MessageMetadataEnvelop<DomainEvent>>();
            //HandlersProcessActor should resend domain event to next step - saga actor - for processing
            ExpectMsg<MessageMetadataEnvelop<DomainEvent>>();
            //HandlersProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<AllHandlersCompleted>();
        }

        [Fact]
        public void Sync_and_async_handlers_performs_independent()
        {
            var fastHandler = Sys.ActorOf(Props.Create(() => new EchoSleepActor(TimeSpan.FromMilliseconds(1), TestActor)));
            var slowHandler = Sys.ActorOf(Props.Create(() => new EchoSleepActor(TimeSpan.FromMilliseconds(100), TestActor)));
            var catalog = new ProcessorListCatalog();

            //Slow handler will receive messages first. 
            //Due to it is registered with async process policy 
            //second handler (fast) will not wait for slow one and it will finish execution before slow handler
            catalog.Add<BalloonCreated>(new FireAndForgetMessageProcessor(slowHandler));
            catalog.Add<BalloonCreated>(new SyncProjectionProcessor(fastHandler));

            catalog.Add<BalloonTitleChanged>(new FireAndForgetMessageProcessor(slowHandler));
            catalog.Add<BalloonTitleChanged>(new SyncProjectionProcessor(fastHandler));

            var actor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(catalog, TestActor)));

            actor.Tell(MessageMetadataEnvelop.New(new Project(new BalloonCreated("1", Guid.NewGuid()),
                                                              new BalloonTitleChanged("1", Guid.NewGuid()))));

            //async handler will last handler to receive all three messages, but will process them faster
            //maintaining initial order
            ExpectMsg<MarkedHandlerExecutedMessage>((e,s) => e.ProcessingMessage.Message is BalloonCreated && e.Mark == fastHandler.Path.ToString(), TimeSpan.FromDays(1));
            ExpectMsg<MarkedHandlerExecutedMessage>((e,s) => e.ProcessingMessage.Message is BalloonTitleChanged && e.Mark == fastHandler.Path.ToString(), TimeSpan.FromDays(1));
            
            //HandlersProcessActor should notify next step - saga actor that work is done
            ExpectMsg<IMessageMetadataEnvelop<DomainEvent>>(e => e.Message is BalloonCreated);
            ExpectMsg<IMessageMetadataEnvelop<DomainEvent>>(e => e.Message is BalloonTitleChanged);

            //HandlersProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<AllHandlersCompleted>();

            //slow async handlers will finish execution only after
            ExpectMsg<MarkedHandlerExecutedMessage>((e, s) => e.ProcessingMessage.Message is BalloonCreated && e.Mark == slowHandler.Path.ToString());
            ExpectMsg<MarkedHandlerExecutedMessage>((e, s) => e.ProcessingMessage.Message is BalloonTitleChanged && e.Mark == slowHandler.Path.ToString());


        }
    }
}