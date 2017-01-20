using System;
using System.Linq;
using System.Threading;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_publishing_start_again : SoftwareProgrammingInstanceSagaTest
    {
        private GotTiredEvent _startMessage;
        private CoffeMadeEvent _coffeMadeEvent;
        private GotTiredEvent _reStartEvent;
        private SagaStateAggregate<SoftwareProgrammingSagaData> _sagaDataAggregate;

        [OneTimeSetUp]
        public void When_publishing_start_message()
        {
            _startMessage = (GotTiredEvent)
               new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())
                   .CloneWithSaga(Guid.NewGuid());

            _coffeMadeEvent = (CoffeMadeEvent)
                new CoffeMadeEvent(_startMessage.FavoriteCoffeMachineId, _startMessage.PersonId)
                    .CloneWithSaga(_startMessage.SagaId);

            _reStartEvent = (GotTiredEvent)
                new GotTiredEvent(Guid.NewGuid(),_startMessage.LovelySofaId, Guid.NewGuid())
                    .CloneWithSaga(_startMessage.SagaId);

            GridNode.Transport.Publish(_startMessage);
            GridNode.Transport.Publish(_coffeMadeEvent);
            GridNode.Transport.Publish(_reStartEvent);

            Thread.Sleep(3000);
         
            _sagaDataAggregate = LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>>(_startMessage.SagaId);
        }

        [Then]
        public void Saga_state_should_be_correct()
        {
            Assert.AreEqual(nameof(SoftwareProgrammingSaga.MakingCoffee), _sagaDataAggregate.Data.CurrentStateName);
        }

        [Then]
        public void Saga_data_contains_information_from_restart_message()
        {
            Assert.AreEqual(_reStartEvent.PersonId, _sagaDataAggregate.Data.PersonId);
        }


        public Given_saga_When_publishing_start_again() : base(true)
        {
        }
    }
}