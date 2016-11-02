using System;
using System.Linq;
using System.Threading;
using Automatonymous;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_uninitialized_saga_When_processing_messages : ProgrammingSoftwareInstanceSagaTest
    {
        private CoffeMadeEvent _coffeMadeEvent;
        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaDataAggregate;

        [TestCase(false, true , Description="Saga id is empty and it has data")]
        [TestCase(false, false, Description = "Saga id is empty and no data")]
        [TestCase(true,  false, Description = "Saga has id and no data")]
        public void Given_saga_when_publishing_known_message_without_saga_data(bool sagaHasId, bool sagaHasData)
        {
            var softwareProgrammingSaga = new SoftwareProgrammingSaga();

            var coffeMadeEvent = new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid());

            var sagaDataAggregate = Aggregate.Empty<SagaDataAggregate<SoftwareProgrammingSagaData>>(!sagaHasId ? Guid.Empty : Guid.NewGuid());
            sagaDataAggregate.RememberEvent(softwareProgrammingSaga.CoffeReady, !sagaHasData ? null : new SoftwareProgrammingSagaData(""), null);

            var saga = SagaInstance.New(softwareProgrammingSaga,sagaDataAggregate);
            saga.Transit(coffeMadeEvent);
            //No exception is raised
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