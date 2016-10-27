using System;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    public class Given_instance_saga_When_exception_on_transit : ProgrammingSoftwareSagaTest
    {
        protected override TimeSpan Timeout { get; } = TimeSpan.FromSeconds(1000);

        [SetUp]
        public void When_saga_receives_a_message_that_case_saga_exception()
        {
            var sagaId = Guid.NewGuid();
            //prepare initial saga state
            var sagaData = new SoftwareProgrammingSagaData(nameof(SoftwareProgrammingSaga.MakingCoffee))
            {
                PersonId = Guid.NewGuid()
            };
            var sagaDataEvent = new SagaCreatedEvent<SoftwareProgrammingSagaData>(sagaData, sagaId);
            SaveInJournal<SagaDataAggregate<SoftwareProgrammingSagaData>>(sagaId, sagaDataEvent);

            Publisher.Publish(new CoffeMakeFailedEvent(Guid.Empty, sagaData.PersonId).CloneWithSaga(sagaId));
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

    public static class ExceptionExtensions
    {
        public static Exception UnwrapInner(this Exception exception)
        {
            return exception.InnerException == null ? exception : UnwrapInner(exception.InnerException);
        }
    }
}