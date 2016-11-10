using System;
using System.Threading;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    class Given_saga_When_publishing_start_messages : SoftwareProgrammingInstanceSagaTest   
    {
        protected readonly Guid _sagaId;
        private readonly object[] _sagaMessages;
        protected SagaDataAggregate<SoftwareProgrammingSagaData> SagaData;
       

        public Given_saga_When_publishing_start_messages(Guid sagaId, params object[] messages)
        {
            _sagaMessages = messages;
            _sagaId = sagaId;

        }
      
        public void When_publishing_start_message()
        {
            foreach(var msg in _sagaMessages)
                GridNode.Transport.Publish(msg);

            Thread.Sleep(Timeout);

            SagaData = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(_sagaId);
        }
    }
}