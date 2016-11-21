using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_publishing_several_start_messages : Given_saga_When_publishing_start_messages
    {
        private static readonly Guid SagaId = Guid.NewGuid();
        private static  GotTiredEvent firstMessage;
        private static  SleptWellEvent secondMessage;
        private static object[] GetMessages(Guid sagaId)
        {
            firstMessage = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), sagaId);
            secondMessage = new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), sagaId);

            return new object[]
            {
                firstMessage,
                secondMessage
            };
        }

        public Given_saga_When_publishing_several_start_messages(): base(SagaId,GetMessages(SagaId))
        {

        }

        [Then]
        public void Saga_reinitialized_from_last_start_message()
        {
            Assert.AreEqual(secondMessage.SofaId, SagaData.Data.SofaId);
        }

        [Then]
        public void Saga_has_correct_state()
        {
            var saga = new SoftwareProgrammingSaga();
            Assert.AreEqual(saga.Coding.Name, SagaData.Data.CurrentStateName);
        }

        protected override IExpectBuilder<AnyMessagePublisher> ConfigureWait(IMessageWaiter<AnyMessagePublisher> waiter)
        {
            return waiter.Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>();
        }
    }
}