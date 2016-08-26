using System;
using System.Threading;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    class Given_saga_When_publishing_any_of_start_messages : ProgrammingSoftwareSagaTest   
    {
        private readonly Guid _sagaId;
        private readonly object[] _sagaMessages;
        protected SagaDataAggregate<SoftwareProgrammingSagaData> SagaData;
       

     
        public Given_saga_When_publishing_any_of_start_messages(Guid sagaId, params object[] messages)
        {
            _sagaMessages = messages;
            _sagaId = sagaId;

        }
      
        [TestFixtureSetUp]
        public void When_publishing_start_message()
        {
            foreach(var msg in _sagaMessages)
                GridNode.Transport.Publish(msg);

            Thread.Sleep(Timeout);

            SagaData = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(_sagaId);
        }

        [Then]
        public void Saga_data_is_not_null()
        {
            Assert.NotNull(SagaData.Data);
        }

        [Then]
        public void Saga_has_correct_state()
        {
            var saga = new SoftwareProgrammingSaga();
            Assert.AreEqual(saga.MakingCoffee.Name,SagaData.Data.CurrentStateName);
        }

        [Then]
        public void Saga_has_correct_id()
        {
            Assert.AreEqual(_sagaId,SagaData.Id);
        }

    }
}