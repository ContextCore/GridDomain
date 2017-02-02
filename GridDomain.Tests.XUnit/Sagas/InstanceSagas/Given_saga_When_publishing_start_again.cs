using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas
{
   
    public class Given_saga_When_publishing_start_again : SoftwareProgrammingInstanceSagaTest
    {
        private GotTiredEvent _startMessage;
        private CoffeMadeEvent _coffeMadeEvent;
        private GotTiredEvent _reStartEvent;
        private SagaStateAggregate<SoftwareProgrammingSagaData> _sagaDataAggregate;

        [Fact]
        public async Task When_publishing_start_message()
        {
            _startMessage = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            _coffeMadeEvent = new CoffeMadeEvent(_startMessage.FavoriteCoffeMachineId, _startMessage.PersonId,null,_startMessage.SagaId);

            _reStartEvent = new GotTiredEvent(Guid.NewGuid(),_startMessage.LovelySofaId, Guid.NewGuid(), _startMessage.SagaId);

            await GridNode.NewDebugWaiter().Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>()
                          .Create()
                          .SendToSagas(_startMessage);

            await GridNode.NewDebugWaiter().Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>()
                          .Create()
                          .SendToSagas(_coffeMadeEvent);

            await GridNode.NewDebugWaiter().Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>()
                          .Create()
                          .SendToSagas(_reStartEvent);

            _sagaDataAggregate = LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>>(_startMessage.SagaId);
         //Saga_state_should_be_correct()
            Assert.Equal(nameof(SoftwareProgrammingSaga.MakingCoffee), _sagaDataAggregate.Data.CurrentStateName);
        //Saga_data_contains_information_from_restart_message()
            Assert.Equal(_reStartEvent.PersonId, _sagaDataAggregate.Data.PersonId);
        }


    }
}