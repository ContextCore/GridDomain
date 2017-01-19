using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.NUnit3;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace GridDomain.Tests.Unit.CommandPipe
{
    [TestFixture]
    class SagaProcessorActorTests : TestKit
    {

        [Test]
        public async Task SagaProcessor_routes_events_by_type()
        {
            var testSagaActor = Sys.ActorOf(Props.Create(() => new TestSagaActor(TestActor, null, null)));

            var catalog = new SagaProcessorCatalog();
            catalog.Add<SampleAggregateCreatedEvent>(new Processor(testSagaActor));

            var sagaProcessActor = Sys.ActorOf(Props.Create(() => new SagaProcessActor(catalog)));
            await sagaProcessActor.Ask<Initialized>(new Initialize(TestActor));


            var msg = new MessageMetadataEnvelop<DomainEvent[]>(new DomainEvent[] { new SampleAggregateCreatedEvent("1", Guid.NewGuid()) },
                                                                MessageMetadata.Empty);

            sagaProcessActor.Tell(msg);

            //TestActor from testSagaActor processor receives message after work is done
            ExpectMsg<SagaTransited>();
            //SagaProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<SagasProcessComplete>();
            //SagaProcessActor should send next step - command execution actor that new commands should be executed
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
        }

        [Test]
        public async Task SagaProcessor_does_not_support_domain_event_inheritance()
        {
            var testSagaActor = Sys.ActorOf(Props.Create(() => new TestSagaActor(TestActor, null, null)));
            var catalog = new SagaProcessorCatalog();
            catalog.Add<SampleAggregateCreatedEvent>(new Processor(testSagaActor));

            var sagaProcessActor = Sys.ActorOf(Props.Create(() => new SagaProcessActor(catalog)));
            await sagaProcessActor.Ask<Initialized>(new Initialize(TestActor));


            var msg = new MessageMetadataEnvelop<DomainEvent[]>(new [] { new DomainEvent(Guid.NewGuid())  },
                                                                MessageMetadata.Empty);

            sagaProcessActor.Tell(msg);

            //saga processor did not run due to error, but we received processing complete message
            ExpectMsg<SagasProcessComplete>();
        }


        [Test]
        public async Task All_Sagas_performs_in_parralel_and_results_from_all_sagas_are_gathered()
        {
            var testSagaActorA = Sys.ActorOf(Props.Create(() => new TestSagaActor(TestActor, null, TimeSpan.FromMilliseconds(50))));

            

            var testSagaActorB = Sys.ActorOf(Props.Create(() => new TestSagaActor(TestActor,
                                                                                  e => new ICommand[] { new TestCommand(e), new TestCommand(e) },
            
                                                                                  TimeSpan.FromMilliseconds(50))));

            var testSagaActorÑ = Sys.ActorOf(Props.Create(() => new TestSagaActor(TestActor, null, TimeSpan.FromMilliseconds(50))));

            var catalog = new SagaProcessorCatalog();

            catalog.Add<SampleAggregateCreatedEvent>(new Processor(testSagaActorA));
            //two commands per one event will be produced
            catalog.Add<SampleAggregateChangedEvent>(new Processor(testSagaActorB));
            catalog.Add<SampleAggregateChangedEvent>(new Processor(testSagaActorÑ));

            var sagaProcessActor = Sys.ActorOf(Props.Create(() => new SagaProcessActor(catalog)));
            await sagaProcessActor.Ask<Initialized>(new Initialize(TestActor));

            var msg = new MessageMetadataEnvelop<DomainEvent[]>(new DomainEvent[]
                                                                {
                                                                    new SampleAggregateCreatedEvent("1", Guid.NewGuid()),
                                                                    new SampleAggregateChangedEvent("2", Guid.NewGuid())
                                                                },
                                                                MessageMetadata.Empty);

            sagaProcessActor.Tell(msg);

            //first we received complete message from all saga actors in undetermined sequence
            ExpectMsg<SagaTransited>();
            ExpectMsg<SagaTransited>();
            ExpectMsg<SagaTransited>();

            //after all sagas complets, SagaProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<SagasProcessComplete>();

            //SagaProcessActor should send all produced commands to execution actor
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
        }

    }
}