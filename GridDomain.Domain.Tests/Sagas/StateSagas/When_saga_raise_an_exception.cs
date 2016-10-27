using System;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    public class When_state_saga_raises_an_exception : SoftwareProgramming_StateSaga_Test
    {
        private Guid _sagaId;
        private Guid _personId;
        
        [SetUp]
        public void When_saga_receives_a_message_that_case_saga_exception()
        {
            _sagaId = Guid.NewGuid();
            _personId = Guid.NewGuid();

            var publisher = GridNode.Container.Resolve<IPublisher>();
            publisher.Publish(new CoffeMakeFailedEvent(Guid.Empty,_personId).CloneWithSaga(_sagaId));
        }

        [Test]
        public void Fault_should_be_produced_and_published()
        {
            WaitFor<IFault<CoffeMakeFailedEvent>>(false);
        }

        [Test]
        public void Fault_should_have_saga_as_producer()
        {
            var fault = (IFault<CoffeMakeFailedEvent>)WaitFor<IFault<CoffeMakeFailedEvent>>(false).Message;
            Assert.AreEqual(fault.Processor, typeof(SoftwareProgrammingSaga));
        }
        [Test]
        public void Fault_should_contains_exception_from_saga()
        {
            var fault = (IFault<CoffeMakeFailedEvent>)WaitFor<IFault<CoffeMakeFailedEvent>>(false).Message;
            Assert.IsInstanceOf<UndefinedCoffeMachineException>(fault.Exception);
        }
    }
}