using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
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
        public async Task When_publishing_start_message()
        {
            var startMessage = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var coffeMadeEvent = new CoffeMadeEvent(startMessage.FavoriteCoffeMachineId,
                                                    startMessage.PersonId,
                                                    null,
                                                    startMessage.SagaId);

            var reStartEvent = new GotTiredEvent(Guid.NewGuid(),
                                                 startMessage.LovelySofaId,
                                                 Guid.NewGuid(),
                                                 startMessage.SagaId);

            await
                Node.NewDebugWaiter()
                    .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaState>>()
                    .Create()
                    .SendToSagas(startMessage);

            await
                Node.NewDebugWaiter()
                    .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaState>>()
                    .Create()
                    .SendToSagas(coffeMadeEvent);

            await
                Node.NewDebugWaiter()
                    .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaState>>()
                    .Create()
                    .SendToSagas(reStartEvent);

            var sagaDataAggregate =
                await this.LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaState>>(startMessage.SagaId);
            //Saga_state_should_be_correct()
            Assert.Equal(nameof(SoftwareProgrammingSaga.MakingCoffee), sagaDataAggregate.Data.CurrentStateName);
            //Saga_data_contains_information_from_restart_message()
            Assert.Equal(reStartEvent.PersonId, sagaDataAggregate.Data.PersonId);
        }
    }
}