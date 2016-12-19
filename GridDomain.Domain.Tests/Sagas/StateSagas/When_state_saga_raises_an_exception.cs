using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using SoftwareProgrammingSaga = GridDomain.Tests.Sagas.StateSagas.SampleSaga.SoftwareProgrammingSaga;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    public class When_state_saga_raises_an_exception : SoftwareProgrammingStateSagaTest
    {
        private IFault<CoffeMakeFailedEvent> _fault;

        //protected override TimeSpan Timeout { get; } = TimeSpan.FromSeconds(3);

        [OneTimeSetUp]
        public async Task When_saga_receives_a_message_that_case_saga_exception()
        {
            var sagaId = Guid.NewGuid();
            var personId = Guid.NewGuid();

            //prepare initial saga state
            
            var sagaDataEvent = new SagaCreatedEvent<SoftwareProgrammingSaga.States>(SoftwareProgrammingSaga.States.MakingCoffee, sagaId);
            SaveInJournal<SoftwareProgrammingSagaState>(sagaId, sagaDataEvent);

            var sagaTransitEvent = new CoffeMakeFailedEvent(Guid.Empty,personId).CloneWithSaga(sagaId);

            var waitResults = await GridNode.NewDebugWaiter()
                                      .Expect<IFault<CoffeMakeFailedEvent>>()
                                      .Create()
                                      .Publish(sagaTransitEvent);

            _fault = waitResults.Message<IFault<CoffeMakeFailedEvent>>();

        }

        [Test]
        public void Fault_should_be_produced_and_published()
        {
           Assert.NotNull(_fault);
        }

        [Test]
        public void Fault_exception_should_contains_stack_trace()
        {
            Assert.True(_fault.Exception.UnwrapInner().StackTrace.Contains(typeof(SoftwareProgrammingSaga).Name));
        }

        [Test]
        public void Fault_should_have_saga_as_producer()
        {
            Assert.AreEqual(typeof(SoftwareProgrammingSaga), _fault.Processor);
        }
        [Test]
        public void Fault_should_contains_exception_from_saga()
        {
            Assert.IsInstanceOf<UndefinedCoffeMachineException>(_fault.Exception.UnwrapInner());
        }
    }
}