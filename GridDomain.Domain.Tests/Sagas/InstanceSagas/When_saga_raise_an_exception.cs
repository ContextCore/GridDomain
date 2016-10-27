using System;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    public class Given_instance_saga_When_exception_on_transit : ProgrammingSoftwareSagaTest
    {
        [SetUp]
        public void When_saga_receives_a_message_that_case_saga_exception()
        {

            var publisher = GridNode.Container.Resolve<IPublisher>();
            publisher.Publish(new CoffeMakeFailedEvent(Guid.Empty,Guid.NewGuid()).CloneWithSaga(Guid.NewGuid()));
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