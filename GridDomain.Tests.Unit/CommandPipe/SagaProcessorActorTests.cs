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

        class TestCommand : Command
        {
            public TestCommand(DomainEvent fromEvent)
            {
                FromEvent = fromEvent;
            }

            public DomainEvent FromEvent { get; }
        }

        class TestSagaActor : ReceiveActor
        {

            public TestSagaActor(IActorRef watcher, 
                                 Func<DomainEvent, ICommand[]> commandFactory = null,
                                 TimeSpan? sleepTime = null)
            {
                var sleep = sleepTime ?? TimeSpan.FromMilliseconds(10);
                commandFactory = commandFactory ?? (e => new ICommand[] { new TestCommand(e)});

                Receive<IMessageMetadataEnvelop<DomainEvent>>(m =>
                {
                    Task.Delay(sleep)
                        .ContinueWith(t => new SagaProcessCompleted(commandFactory(m.Message),m.Metadata))
                        .PipeTo(Self, Sender);
                });
                    

                Receive<SagaProcessCompleted>(m =>
                {
                    watcher.Tell(m);
                    Sender.Tell(m);
                });
            }
        }


        [Test]
        public void SagaProcessor_routes_events_by_type()
        {
            var testSagaActor = Sys.ActorOf(Props.Create(() => new TestSagaActor(TestActor, null, null)));
            var catalog = new SagaProcessorCatalog();
            catalog.Add<SampleAggregateCreatedEvent>(new Processor(testSagaActor));

            var sagaProcessActor = Sys.ActorOf(Props.Create(() => new SagaProcessActor(catalog, TestActor)));

            var msg = new MessageMetadataEnvelop<DomainEvent[]>(new DomainEvent[] { new SampleAggregateCreatedEvent("1", Guid.NewGuid()) },
                                                                MessageMetadata.Empty());

            sagaProcessActor.Tell(msg);

            //TestActor from testSagaActor processor receives message after work is done
            ExpectMsg<SagaProcessCompleted>();
            //SagaProcessActor should notify sender (TestActor) of initial messages that work is done
            ExpectMsg<SagasProcessComplete>();
            //SagaProcessActor should send next step - command execution actor that new commands should be executed
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
        }

        [Test]
        public void SagaProcessor_does_not_support_domain_event_inheritance()
        {
            var testSagaActor = Sys.ActorOf(Props.Create(() => new TestSagaActor(TestActor, null, null)));
            var catalog = new SagaProcessorCatalog();
            catalog.Add<SampleAggregateCreatedEvent>(new Processor(testSagaActor));

            var sagaProcessActor = Sys.ActorOf(Props.Create(() => new SagaProcessActor(catalog, TestActor)));

            var msg = new MessageMetadataEnvelop<DomainEvent[]>(new [] { new DomainEvent(Guid.NewGuid())  },
                                                                MessageMetadata.Empty());

            sagaProcessActor.Tell(msg);

            ExpectNoMsg();
        }


        [Test]
        public void All_Sagas_performs_in_parralel_and_results_from_all_sagas_are_gathered()
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

            var sagaProcessActor = Sys.ActorOf(Props.Create(() => new SagaProcessActor(catalog, TestActor)));

            var msg = new MessageMetadataEnvelop<DomainEvent[]>(new DomainEvent[]
                                                                {
                                                                    new SampleAggregateCreatedEvent("1", Guid.NewGuid()),
                                                                    new SampleAggregateChangedEvent("2", Guid.NewGuid())
                                                                },
                                                                MessageMetadata.Empty());

            sagaProcessActor.Tell(msg);

            //first we received complete message from all saga actors in undetermined sequence
            ExpectMsg<SagaProcessCompleted>();
            ExpectMsg<SagaProcessCompleted>();
            ExpectMsg<SagaProcessCompleted>();

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