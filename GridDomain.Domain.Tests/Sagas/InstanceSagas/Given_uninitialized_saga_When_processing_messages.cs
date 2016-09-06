using System;
using System.Linq;
using System.Threading;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.FutureEvents;
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

        [Test]
        public void When_publishing_known_message_without_saga_data()
        {
           var  coffeMadeEvent = new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid());

            var saga = SagaInstance.New(new SoftwareProgrammingSaga(),
                Aggregate.Empty<SagaDataAggregate<SoftwareProgrammingSagaData>>(Guid.Empty));

           saga.Transit(coffeMadeEvent);
           Assert.AreEqual(Guid.Empty, saga.Data.Id);
        }

        [Then]
        public void Saga_data_should_not_be_changed()
        {
            _coffeMadeEvent = (CoffeMadeEvent)
               new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid()).CloneWithSaga(Guid.NewGuid());

            GridNode.Transport.Publish(_coffeMadeEvent);
            Thread.Sleep(200);
            _sagaDataAggregate = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(_coffeMadeEvent.SagaId);
            Assert.Null(_sagaDataAggregate.Data);
        }

        public Given_uninitialized_saga_When_processing_messages() : base(true)
        {
        }
    }
}