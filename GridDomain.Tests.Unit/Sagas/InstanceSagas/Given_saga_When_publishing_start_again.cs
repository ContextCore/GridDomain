using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
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
        public async Task When_publishing_start_message()
        {
            _startMessage = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            _coffeMadeEvent = new CoffeMadeEvent(_startMessage.FavoriteCoffeMachineId, _startMessage.PersonId,null,_startMessage.SagaId);

            _reStartEvent = new GotTiredEvent(Guid.NewGuid(),_startMessage.LovelySofaId, Guid.NewGuid(), _startMessage.SagaId);

            await GridNode.NewDebugWaiter().Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>()
                          .Create()
                          .SendToSaga(_startMessage);

            await GridNode.NewDebugWaiter().Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>()
                          .Create()
                          .SendToSaga(_coffeMadeEvent);

            await GridNode.NewDebugWaiter().Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>()
                          .Create()
                          .SendToSaga(_reStartEvent);

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