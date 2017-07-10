using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.CommandPipe.Processors;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;

namespace GridDomain.Tests.Unit.CommandPipe
{
    public class SagaProcessorActorTests : TestKit
    {
        [Fact]
        public async Task All_Sagas_performs_in_parralel_and_results_from_all_sagas_are_gathered()
        {
            var testSagaActorA =
                Sys.ActorOf(Props.Create(() => new TestSagaActor(TestActor, null, TimeSpan.FromMilliseconds(50))));


            var testSagaActorB =
                Sys.ActorOf(
                            Props.Create(
                                         () =>
                                             new TestSagaActor(TestActor,
                                                               e => new ICommand[] {new TestCommand(e.SourceId), new TestCommand(e.SourceId)},
                                                               TimeSpan.FromMilliseconds(50))));

            var testSagaActorÑ =
                Sys.ActorOf(Props.Create(() => new TestSagaActor(TestActor, null, TimeSpan.FromMilliseconds(50))));

            var catalog = new ProcessorListCatalog();

            catalog.Add<BalloonCreated>(new SyncProjectionProcessor(testSagaActorA));
            //two commands per one event will be produced
            catalog.Add<BalloonTitleChanged>(new SyncProjectionProcessor(testSagaActorB));
            catalog.Add<BalloonTitleChanged>(new SyncProjectionProcessor(testSagaActorÑ));

            var sagaProcessActor = Sys.ActorOf(Props.Create(() => new SagaPipeActor(catalog)));
            await sagaProcessActor.Ask<Initialized>(new Initialize(TestActor));

            sagaProcessActor.Tell(MessageMetadataEnvelop.New<DomainEvent>(new BalloonCreated("1", Guid.NewGuid())));
            sagaProcessActor.Tell(MessageMetadataEnvelop.New<DomainEvent>(new BalloonTitleChanged("2", Guid.NewGuid())));

            //first we received complete message from all saga actors in undetermined sequence
            ExpectMsg<SagaTransited>();
            ExpectMsg<SagaTransited>();
            ExpectMsg<SagaTransited>();

            //after all sagas complets, SagaProcessActor should notify sender (TestActor) of initial messages that work is done
            //ExpectMsg<SagasProcessComplete>();

            //SagaProcessActor should send all produced commands to execution actor
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
        }



        class Inherited : BalloonCreated
        {
            public Inherited():base("123",Guid.NewGuid())
            {
                
            }
        }
        [Fact]
        public async Task SagaProcessor_does_not_support_domain_event_inheritance()
        {
            var testSagaActor = Sys.ActorOf(Props.Create(() => new TestSagaActor(TestActor, null, null)));
            var catalog = new ProcessorListCatalog();
            catalog.Add<BalloonCreated>(new SyncSagaProcessor(testSagaActor));

            var sagaProcessActor = Sys.ActorOf(Props.Create(() => new SagaPipeActor(catalog)));
            await sagaProcessActor.Ask<Initialized>(new Initialize(TestActor));

            var msg = MessageMetadataEnvelop.New<DomainEvent>(new Inherited());

            sagaProcessActor.Tell(msg);

            //saga processor did not run due to error, but we received processing complete message
            //ExpectMsg<SagasProcessComplete>();
            ExpectNoMsg(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task SagaProcessor_routes_events_by_type()
        {
            var testSagaActor = Sys.ActorOf(Props.Create(() => new TestSagaActor(TestActor, null, null)));

            var catalog = new ProcessorListCatalog();
            catalog.Add<BalloonCreated>(new SyncSagaProcessor(testSagaActor));

            var sagaProcessActor = Sys.ActorOf(Props.Create(() => new SagaPipeActor(catalog)));
            await sagaProcessActor.Ask<Initialized>(new Initialize(TestActor));


            var msg =
                new MessageMetadataEnvelop<DomainEvent>(new BalloonCreated("1", Guid.NewGuid()),
                                                        MessageMetadata.Empty);

            sagaProcessActor.Tell(msg);

            //TestActor from testSagaActor processor receives message after work is done
            ExpectMsg<SagaTransited>();
            //SagaProcessActor should notify sender (TestActor) of initial messages that work is done
            //ExpectMsg<SagasProcessComplete>();
            //SagaProcessActor should send next step - command execution actor that new commands should be executed
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
        }
    }
}