using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class Given_saga_When_publishing_start_again : SoftwareProgrammingSagaTest
    {
        public Given_saga_When_publishing_start_again(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task When_publishing_start_message_for_new_saga()
        {
            var startMessage = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var res = await Node.NewDebugWaiter()
                                .Expect<SagaCreatedEvent<SoftwareProgrammingSagaState>>()
                                .Create()
                                .SendToSagas(startMessage);

            var newSagaId = res.Message<SagaCreatedEvent<SoftwareProgrammingSagaState>>().Id;

            var coffeMadeEvent = new CoffeMadeEvent(startMessage.FavoriteCoffeMachineId,
                                                    startMessage.PersonId,
                                                    null,
                                                    newSagaId);

            await Node.NewDebugWaiter()
                      .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaState>>()
                      .Create()
                      .SendToSagas(coffeMadeEvent);


            var reStartEvent = new GotTiredEvent(Guid.NewGuid(),
                                                 startMessage.LovelySofaId,
                                                 Guid.NewGuid(),
                                                 newSagaId);

            await Node.NewDebugWaiter()
                      .Expect<SagaCreatedEvent<SoftwareProgrammingSagaState>>()
                      .Create()
                      .SendToSagas(reStartEvent);

            var sagaState = await this.LoadSaga<SoftwareProgrammingSagaState>(newSagaId);
            //Saga_state_should_be_correct()
            Assert.Equal(nameof(SoftwareProgrammingSaga.MakingCoffee), sagaState.CurrentStateName);
            //Saga_data_contains_information_from_restart_message()
            Assert.Equal(reStartEvent.PersonId, sagaState.PersonId);
        }
    }
}