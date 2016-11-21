using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{


    [TestFixture]
    class Given_saga_When_publishing_any_of_start_messages : Given_saga_When_publishing_start_messages
    {
        private static Guid SagaId = Guid.NewGuid();

        public Given_saga_When_publishing_any_of_start_messages():
            base(SagaId, new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), SagaId))
        {
            
        }

        [Then]
        public void Saga_data_is_not_null()
        {
            Assert.NotNull(SagaData.Data);
        }


        [Then]
        public void Saga_has_correct_id()
        {
            Assert.AreEqual(_sagaId, SagaData.Id);
        }

        protected override IExpectBuilder<AnyMessagePublisher> ConfigureWait(IMessageWaiter<AnyMessagePublisher> waiter)
        {
            return waiter.Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>();
        }
    }
}