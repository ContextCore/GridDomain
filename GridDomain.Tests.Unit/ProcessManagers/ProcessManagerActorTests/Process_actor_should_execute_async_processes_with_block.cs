using System;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Akka.TestKit;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using Autofac;
using GridDomain.Common;

using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.AkkaMessaging;
using GridDomain.ProcessManagers.Creation;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Transport;
using GridDomain.Transport.Extension;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers.ProcessManagerActorTests
{
    public class Process_actor_should_execute_async_processes_with_block : TestKit
    {
        public Process_actor_should_execute_async_processes_with_block(ITestOutputHelper output)
        {
            var logger =  new XUnitAutoTestLoggerConfiguration(output).CreateLogger(); 
            var creator = new AsyncLongRunningProcessManagerFactory(logger);
            var producer = new ProcessManager—reatorsCatalog<TestState>(AsyncLongRunningProcess.Descriptor, creator);
            producer.RegisterAll(creator);

            _localAkkaEventBusTransport = Sys.InitLocalTransportExtension().Transport;
            var blackHole = Sys.ActorOf(BlackHoleActor.Props);

            var messageProcessActor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(new ProcessorListCatalog(), blackHole)));
            _processId = Guid.NewGuid();

            var container = new ContainerBuilder();

            container.Register<ProcessStateActor<TestState>>(c =>
                                                             new ProcessStateActor<TestState>(CommandAggregateHandler.New<ProcessStateAggregate<TestState>>(),
                                                                                              new EachMessageSnapshotsPersistencePolicy(), new AggregateFactory(),
                                                                                              messageProcessActor));
            Sys.AddDependencyResolver(new AutoFacDependencyResolver(container.Build(), Sys));

            var name = AggregateActorName.New<ProcessStateAggregate<TestState>>(_processId).Name;
            _processActor = ActorOfAsTestActorRef(() => new ProcessActor<TestState>(producer),
                                                  name);
        }

        private readonly TestActorRef<ProcessManagerActor<TestState>> _processActor;
        private readonly Guid _processId;
        private readonly IActorTransport _localAkkaEventBusTransport;

        [Fact]
        public void Process_actor_process_one_message_in_time()
        {
            var domainEventA = new BalloonCreated("1", Guid.NewGuid(), DateTime.Now, _processId);
            var domainEventB = new BalloonTitleChanged("2", Guid.NewGuid(), DateTime.Now, _processId);

            _processActor.Tell(MessageMetadataEnvelop.New(domainEventA, MessageMetadata.Empty));
            _processActor.Tell(MessageMetadataEnvelop.New(domainEventB, MessageMetadata.Empty));

            //A was received first and should be processed first
            var msg = ExpectMsg<ProcessTransited>();
            Assert.Equal(domainEventA.SourceId,((TestState)msg.NewProcessState).ProcessingId);
            //B should not be processed after A is completed
            var msgB = ExpectMsg<ProcessTransited>();
            Assert.Equal(domainEventB.SourceId,((TestState)msgB.NewProcessState).ProcessingId);
        }

        [Fact]
        public void Process_change_state_after_transitions()
        {
            var domainEventA = new BalloonCreated("1", Guid.NewGuid(), DateTime.Now, _processId);

            _processActor.Ref.Tell(MessageMetadataEnvelop.New(domainEventA));

            var msg = ExpectMsg<ProcessTransited>();

            Assert.Equal(domainEventA.SourceId, ((TestState)msg.NewProcessState).ProcessingId);
        }

        [Fact]
        public void Process_transition_raises_state_events()
        {
            _processActor.Ref.Tell(MessageMetadataEnvelop.New(new BalloonCreated("1", Guid.NewGuid(), DateTime.Now, _processId),
                                                           MessageMetadata.Empty));

            _localAkkaEventBusTransport.Subscribe(typeof(IMessageMetadataEnvelop), TestActor);

            FishForMessage<MessageMetadataEnvelop>(m => m.Message is ProcessManagerCreated<TestState>);
            FishForMessage<MessageMetadataEnvelop>(m => m.Message is ProcessReceivedMessage<TestState>);
        }
    }
}