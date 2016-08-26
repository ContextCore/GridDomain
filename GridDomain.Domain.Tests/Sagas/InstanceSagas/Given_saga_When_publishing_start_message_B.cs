using System;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_publishing_start_message_B : Given_saga_When_publishing_start_messages
    {
        private static readonly SleptWellEvent StartMessage = new SleptWellEvent(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(), null);

        public Given_saga_When_publishing_start_message_B() : base(StartMessage.SagaId, StartMessage)
        {


        }

        [TestFixtureSetUp]
        public void Setup()
        {
            base.When_publishing_start_message();
        }

        [Then]
        public void Saga_has_correct_data()
        {
            Assert.AreEqual(StartMessage.SofaId, SagaData.Data.SofaId);
        }

        [Then]
        public void Saga_has_correct_state()
        {
            var saga = new SoftwareProgrammingSaga();
            Assert.AreEqual(saga.Coding.Name, SagaData.Data.CurrentStateName);
        }
    }
}