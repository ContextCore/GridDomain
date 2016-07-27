using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    internal class SubscriptionRenewSagaStateTransitionTests
    {
        private SoftwareProgrammingSaga SagaInstance;

        public void Given_new_saga_with_state(SoftwareProgrammingSaga.States states)
        {
            var sagaState = new SoftwareProgrammingSagaState(Guid.NewGuid(), states);
            SagaInstance = new SoftwareProgrammingSaga(sagaState);
        }

        private class WrongMessage
        {
        }


        [Test]
        public void When_invalid_transition_Then_state_not_changed()
        {
            Given_new_saga_with_state(SoftwareProgrammingSaga.States.Sleeping);
            SagaInstance.Transit(new GotTiredEvent(Guid.NewGuid()));

            Assert.AreEqual(SoftwareProgrammingSaga.States.Sleeping, SagaInstance.DomainState);
        }

        [Test]
        public void When_unknown_transition_Then_exception_occurs()
        {
            Given_new_saga_with_state(SoftwareProgrammingSaga.States.Sleeping);
            Assert.Throws<UnbindedMessageReceivedException>(() => SagaInstance.TransitState(new WrongMessage()));
        }


        [Test]
        public void When_valid_transition_Then_state_is_changed()
        {
            Given_new_saga_with_state(SoftwareProgrammingSaga.States.MakingCoffe);
            SagaInstance.Handle(new CoffeMadeEvent(Guid.NewGuid(),Guid.NewGuid()));

            Assert.AreEqual(SoftwareProgrammingSaga.States.Working, SagaInstance.DomainState);
        }
    }
}