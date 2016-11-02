using System;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using SoftwareProgrammingSaga = GridDomain.Tests.Sagas.StateSagas.SampleSaga.SoftwareProgrammingSaga;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    public class When_state_saga_raises_an_exception : ProgrammingSoftwareStateSagaTest
    {
        protected override TimeSpan Timeout { get; } = TimeSpan.FromSeconds(3);

        [SetUp]
        public void When_saga_receives_a_message_that_case_saga_exception()
        {
            var sagaId = Guid.NewGuid();
            var personId = Guid.NewGuid();

            //prepare initial saga state
            
            var sagaDataEvent = new SagaCreatedEvent<SoftwareProgrammingSaga.States>(SoftwareProgrammingSaga.States.MakingCoffe, sagaId);
            SaveInJournal<SoftwareProgrammingSagaState>(sagaId, sagaDataEvent);

            Publisher.Publish(new CoffeMakeFailedEvent(Guid.Empty,personId).CloneWithSaga(sagaId));
        }

        [Test]
        public void Fault_should_be_produced_and_published()
        {
            WaitFor<IFault<CoffeMakeFailedEvent>>(false);
        }

        [Test]
        public void Fault_exception_should_contains_stack_trace()
        {
            var fault = (IFault<CoffeMakeFailedEvent>)WaitFor<IFault<CoffeMakeFailedEvent>>(false).Message;
            Assert.True(fault.Exception.UnwrapInner().StackTrace.Contains(typeof(SoftwareProgrammingSaga).Name));
        }

        [Test]
        public void Fault_should_have_saga_as_producer()
        {
            var fault = (IFault<CoffeMakeFailedEvent>)WaitFor<IFault<CoffeMakeFailedEvent>>(false).Message;
            Assert.AreEqual(typeof(SoftwareProgrammingSaga), fault.Processor);
        }
        [Test]
        public void Fault_should_contains_exception_from_saga()
        {
            var fault = (IFault<CoffeMakeFailedEvent>)WaitFor<IFault<CoffeMakeFailedEvent>>(false).Message;
            Assert.IsInstanceOf<UndefinedCoffeMachineException>(fault.Exception.UnwrapInner());
        }
    }
}