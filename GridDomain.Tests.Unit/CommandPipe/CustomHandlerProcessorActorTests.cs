using System;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.Hadlers;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Transport.Extension;
using Xunit;

namespace GridDomain.Tests.Unit.CommandPipe
{
    public class CustomHandlerProcessorActorTests : TestKit
    {
        [Fact]
        public void All_async_handlers_performs_in_parralel()
        {
            Sys.InitLocalTransportExtension();
            var delayActor = Sys.ActorOf(Props.Create(() => new EchoSleepActor(TimeSpan.FromMilliseconds(100), TestActor)));
            var catalog = new HandlersDefaultProcessor();

            catalog.Add<BalloonCreated>(new FireAndForgetActorMessageProcessor(delayActor));
            catalog.Add<BalloonTitleChanged>(new FireAndForgetActorMessageProcessor(delayActor));

            var actor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(catalog, TestActor.Path.ToString())));

            var sampleAggregateCreatedEvent = new BalloonCreated("1", Guid.NewGuid().ToString());
            var sampleAggregateChangedEvent = new BalloonTitleChanged("1", Guid.NewGuid().ToString());

            actor.Tell(new HandlersPipeActor.Project(TestActor,new MessageMetadataEnvelop(sampleAggregateCreatedEvent)));

            //HandlersProcessActor should notify next step - process actor that work is done
            ExpectMsg<IMessageMetadataEnvelop>(m => m.Message is DomainEvent);
            ExpectMsg<AllHandlersCompleted>();
            //but handlers will finish their work later in undefined sequence

            actor.Tell(new HandlersPipeActor.Project(TestActor,MessageMetadata.Empty,sampleAggregateChangedEvent));

            ExpectMsg<IMessageMetadataEnvelop>(m => m.Message is DomainEvent);
            //HandlersProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<AllHandlersCompleted>();

        }


        [Fact]
        public void Given_no_processors_pipe_still_reply_with_completed_messages()
        {
            var catalog = new HandlersDefaultProcessor();
            Sys.InitLocalTransportExtension();
            var actor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(catalog,  TestActor.Path.ToString())));

            var sampleAggregateCreatedEvent = new BalloonCreated("1", Guid.NewGuid().ToString());

            actor.Tell(new HandlersPipeActor.Project(TestActor,MessageMetadataEnvelop.New(sampleAggregateCreatedEvent)));


            //HandlersPipeActor should notify next step - process actor that work is done
            ExpectMsg<IMessageMetadataEnvelop<DomainEvent>>();
            //HandlersPipeActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<AllHandlersCompleted>();
        }

        [Fact]
        public void All_sync_handlers_performs_one_after_one()
        {
            Sys.InitLocalTransportExtension();
            var delayActor = Sys.ActorOf(Props.Create(() => new EchoSleepActor(TimeSpan.FromMilliseconds(50), TestActor)));
            var catalog = new HandlersDefaultProcessor();

            catalog.Add<BalloonCreated>(new SyncProjectionProcessor(delayActor));
            catalog.Add<BalloonTitleChanged>(new SyncProjectionProcessor(delayActor));
            catalog.Add<BalloonTitleChanged>(new SyncProjectionProcessor(delayActor));

            var actor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(catalog, TestActor.Path.ToString())));


            var messageMetadataEnvelop = MessageMetadataEnvelop.New(new BalloonCreated("1", Guid.NewGuid().ToString()));
            actor.Tell(new HandlersPipeActor.Project(TestActor,messageMetadataEnvelop));

            //in sync process we should wait for handlers execution
            //in same order as they were sent to handlers process actor
            ExpectMsg<MarkedHandlerExecutedMessage>();
            ExpectMsg<IMessageMetadataEnvelop<DomainEvent>>();
            //HandlersProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<AllHandlersCompleted>();

            var metadataEnvelop = MessageMetadataEnvelop.New(new BalloonTitleChanged("2", Guid.NewGuid().ToString()));
            
            actor.Tell(new HandlersPipeActor.Project(TestActor,metadataEnvelop));

            ExpectMsg<MarkedHandlerExecutedMessage>();
            ExpectMsg<MarkedHandlerExecutedMessage>();
            //HandlersProcessActor should notify next step - process actor that work is done
            ExpectMsg<IMessageMetadataEnvelop<DomainEvent>>();
            ExpectMsg<AllHandlersCompleted>();



        }

        class Inherited : BalloonCreated
        {
            public Inherited() : base("inherited", Guid.NewGuid().ToString()) { }
        }

        [Fact]
        public void CustomHandlerExecutor_does_not_support_domain_event_inheritance()
        {
            Sys.InitLocalTransportExtension();

            var catalog = new HandlersDefaultProcessor();
            catalog.Add<BalloonCreated>(new SyncProjectionProcessor(TestActor));
            var actor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(catalog, TestActor.Path.ToString())));

            var msg = MessageMetadataEnvelop.New(new Inherited());

            actor.Tell(new HandlersPipeActor.Project(TestActor,msg));
            //processor did not run, but we pass message to process after
            ExpectMsg<MessageMetadataEnvelop<Inherited>>();
            //processor did not run, but we received processing complete message
            ExpectMsg<AllHandlersCompleted>();
        }

        [Fact]
        public void CustomHandlerProcessor_routes_events_by_type()
        {
            Sys.InitLocalTransportExtension();

            var catalog = new HandlersDefaultProcessor();
            catalog.Add<BalloonCreated>(new FireAndForgetActorMessageProcessor(TestActor));
            var actor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(catalog, TestActor.Path.ToString())));

            var messageMetadataEnvelop = MessageMetadataEnvelop.New(new BalloonCreated("1", Guid.NewGuid().ToString()));
            actor.Tell(new HandlersPipeActor.Project(TestActor,messageMetadataEnvelop));
           
            //TestActor as processor receives message for work
            ExpectMsg<MessageMetadataEnvelop<BalloonCreated>>();
            //HandlersProcessActor should resend domain event to next step - process actor - for processing
            ExpectMsg<MessageMetadataEnvelop<BalloonCreated>>();
            //HandlersProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<AllHandlersCompleted>();
        }

        [Fact]
        public void Sync_and_async_handlers_performs_independent()
        {
            Sys.InitLocalTransportExtension();

            var fastHandler = Sys.ActorOf(Props.Create(() => new EchoSleepActor(TimeSpan.FromMilliseconds(1), TestActor)),"fastHandler");
            var slowHandler = Sys.ActorOf(Props.Create(() => new EchoSleepActor(TimeSpan.FromMilliseconds(1000), TestActor)),"slowHandler");
            var catalog = new HandlersDefaultProcessor();

            //Slow handler will receive messages first. 
            //Due to it is registered with async process policy 
            //second handler (fast) will not wait for slow one and it will finish execution before slow handler
            catalog.Add<BalloonCreated>(new FireAndForgetActorMessageProcessor(slowHandler));
            catalog.Add<BalloonCreated>(new SyncProjectionProcessor(fastHandler));

            catalog.Add<BalloonTitleChanged>(new FireAndForgetActorMessageProcessor(slowHandler));
            catalog.Add<BalloonTitleChanged>(new SyncProjectionProcessor(fastHandler));

            var actor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(catalog, TestActor.Path.ToString())));

            actor.Tell(new HandlersPipeActor.Project(TestActor, MessageMetadataEnvelop.New(new BalloonCreated("1", Guid.NewGuid().ToString()))));
            actor.Tell(new HandlersPipeActor.Project(TestActor, MessageMetadataEnvelop.New(new BalloonTitleChanged("1", Guid.NewGuid().ToString()))));

            FishForMessage<MarkedHandlerExecutedMessage>(e => e.Mark == "fastHandler");
            FishForMessage<MarkedHandlerExecutedMessage>(e => e.Mark == "fastHandler");
            
            //slow fire and forget handlers will finish execution in undetermined order

            FishForMessage<MarkedHandlerExecutedMessage>(e => e.Mark == "slowHandler");
            FishForMessage<MarkedHandlerExecutedMessage>(e =>e.Mark == "slowHandler");
        }
    }
}