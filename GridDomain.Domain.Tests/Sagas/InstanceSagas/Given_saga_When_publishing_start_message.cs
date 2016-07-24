using System;
using System.Threading;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_publishing_start_message : ProgrammingSoftwareSagaTest   
{
        private Guid _sagaId;
        private GotTiredEvent _sagaStartMessage;
        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaData;
    
        [TestFixtureSetUp]
        public void When_publishing_start_message()
        {
            _sagaStartMessage = (GotTiredEvent)
                new GotTiredEvent(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid())
                                         .CloneWithSaga(Guid.NewGuid());

            _sagaId = _sagaStartMessage.SagaId;
            GridNode.Transport.Publish(_sagaStartMessage);

            Thread.Sleep(Timeout);

            _sagaData = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(_sagaId);
        }

        [Then]
        public void Saga_data_is_not_null()
        {
            Assert.NotNull(_sagaData.Data);
        }

        [Then]
        public void Saga_has_correct_state()
        {
            var saga = new SoftwareProgrammingSaga();
            Assert.AreEqual(saga.MakingCoffee.Name,_sagaData.Data.CurrentStateName);
        }

        [Then]
        public void Saga_has_correct_id()
        {
            Assert.AreEqual(_sagaStartMessage.SagaId,_sagaData.Id);
        }

        [Then]
        public void Saga_has_correct_data()
        {
            Assert.AreEqual(_sagaStartMessage.PersonId, _sagaData.Data.PersonId);
        }
    }
}
