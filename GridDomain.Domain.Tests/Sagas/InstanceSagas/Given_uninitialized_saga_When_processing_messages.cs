using System;
using System.Linq;
using System.Threading;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_uninitialized_saga_When_processing_messages : ProgrammingSoftwareSagaTest
    {
        private CoffeMadeEvent _coffeMadeEvent;
        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaDataAggregate;

        [TestFixtureSetUp]
        public void When_publishing_not_start_known_message()
        {
            _coffeMadeEvent = (CoffeMadeEvent)
                new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid()).CloneWithSaga(Guid.NewGuid());
            
            GridNode.Transport.Publish(_coffeMadeEvent);
            Thread.Sleep(200);
            _sagaDataAggregate = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(_coffeMadeEvent.SagaId);
        }

        [Then]
        public void Saga_data_should_not_be_changed()
        {
            Assert.Null(_sagaDataAggregate.Data);
        }

        public Given_uninitialized_saga_When_processing_messages() : base(true)
        {
        }
    }
}