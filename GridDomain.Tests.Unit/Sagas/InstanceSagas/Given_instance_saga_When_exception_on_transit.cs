using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    [TestFixture]
    public class Given_instance_saga_When_exception_on_transit : SoftwareProgrammingInstanceSagaTest
    {
        private IFault<CoffeMakeFailedEvent> _fault;

        [OneTimeSetUp]
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

            var waiter = GridNode.NewWaiter(TimeSpan.FromSeconds(1000))
                                 .Expect<IFault<CoffeMakeFailedEvent>>()
                                 .Create();

            GridNode.Transport.Publish(new CoffeMakeFailedEvent(Guid.Empty, sagaData.PersonId).CloneWithSaga(sagaId));

            _fault = waiter.Result.Message<IFault<CoffeMakeFailedEvent>>();
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