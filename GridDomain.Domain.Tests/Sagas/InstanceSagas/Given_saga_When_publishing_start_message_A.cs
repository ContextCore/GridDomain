using System;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_publishing_start_message_A : Given_saga_When_publishing_start_messages
    {
        private static readonly GotTiredEvent StartMessage = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid());
        public Given_saga_When_publishing_start_message_A():base(StartMessage.SagaId, StartMessage)
        {
            

        }

        [OneTimeSetUp]
        public void Setup()
        {
            base.When_publishing_start_message();
        }


        [Then]
        public void Saga_has_correct_data()
        {
            Assert.AreEqual(StartMessage.PersonId, SagaData.Data.PersonId);
        }

        [Then]
        public void Saga_has_correct_state()
        {
            var saga = new SoftwareProgrammingSaga();
            Assert.AreEqual(saga.MakingCoffee.Name, SagaData.Data.CurrentStateName);
        }
    }
}
