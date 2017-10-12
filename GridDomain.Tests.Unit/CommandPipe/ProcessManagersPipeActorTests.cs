using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;

namespace GridDomain.Tests.Unit.CommandPipe
{
    public class ProcessManagersPipeActorTests : TestKit
    {
        [Fact]
        public async Task All_Processes_performs_in_parralel_and_results_from_all_processes_are_gathered()
        {
            var testProcessActorA =
                Sys.ActorOf(Props.Create(() => new TestProcessActor(TestActor, null, TimeSpan.FromMilliseconds(50))));


            var testProcessActorB =
                Sys.ActorOf(
                            Props.Create(
                                         () =>
                                             new TestProcessActor(TestActor,
                                                               e => new ICommand[] {new TestCommand(e.SourceId), new TestCommand(e.SourceId)},
                                                               TimeSpan.FromMilliseconds(50))));

            var testProcessActorC =
                Sys.ActorOf(Props.Create(() => new TestProcessActor(TestActor, null, TimeSpan.FromMilliseconds(50))));

            var catalog = new ProcessorListCatalog<IProcessCompleted>();

            catalog.Add<BalloonCreated>(new SyncProcessManagerProcessor(testProcessActorA));
            //two commands per one event will be produced
            catalog.Add<BalloonTitleChanged>(new SyncProcessManagerProcessor(testProcessActorB));
            catalog.Add<BalloonTitleChanged>(new SyncProcessManagerProcessor(testProcessActorC));

            var processPipeActor = Sys.ActorOf(Props.Create(() => new ProcessesPipeActor(catalog)));
            await processPipeActor.Ask<Initialized>(new Initialize(TestActor));

            processPipeActor.Tell(MessageMetadataEnvelop.New<DomainEvent>(new BalloonCreated("1", Guid.NewGuid())));
            processPipeActor.Tell(MessageMetadataEnvelop.New<DomainEvent>(new BalloonTitleChanged("2", Guid.NewGuid())));

            //first we received complete message from all process actors in undetermined sequence
            ExpectMsg<ProcessTransited>();
            ExpectMsg<ProcessTransited>();
            ExpectMsg<ProcessTransited>();

            //after all process managers complets, process pipe actor should notify sender (TestActor) of initial messages that work is done

            //process managers pipe actor should send all produced commands to execution actor
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
        }

        class Inherited : BalloonCreated
        {
            public Inherited() : base("123", Guid.NewGuid()) { }
        }

        [Fact]
        public async Task ProcessManagerPipeActor_does_not_support_domain_event_inheritance()
        {
            var testProcessActor = Sys.ActorOf(Props.Create(() => new TestProcessActor(TestActor, null, null)));
            var catalog = new ProcessorListCatalog<IProcessCompleted>();
            catalog.Add<BalloonCreated>(new SyncProcessManagerProcessor(testProcessActor));

            var processPipeActor = Sys.ActorOf(Props.Create(() => new ProcessesPipeActor(catalog)));
            await processPipeActor.Ask<Initialized>(new Initialize(TestActor));

            var msg = MessageMetadataEnvelop.New<DomainEvent>(new Inherited());

            processPipeActor.Tell(msg);

            //process processor did not run due to error, but we received processing complete message
            ExpectNoMsg(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ProcessPipeActor_routes_events_by_type()
        {
            var testProcessActor = Sys.ActorOf(Props.Create(() => new TestProcessActor(TestActor, null, null)));

            var catalog = new ProcessorListCatalog<IProcessCompleted>();
            catalog.Add<BalloonCreated>(new SyncProcessManagerProcessor(testProcessActor));

            var processPipeActor = Sys.ActorOf(Props.Create(() => new ProcessesPipeActor(catalog)));
            await processPipeActor.Ask<Initialized>(new Initialize(TestActor));


            var msg = new MessageMetadataEnvelop<DomainEvent>(new BalloonCreated("1", Guid.NewGuid()),
                                                              MessageMetadata.Empty);

            processPipeActor.Tell(msg);

            //TestActor from test process processor receives message after work is done
            ExpectMsg<ProcessTransited>();
            //process pipe should send next step - command execution actor that new commands should be executed
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
        }
    }
}