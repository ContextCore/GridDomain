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
    public class Given_saga_When_publishing_start_message_B : SoftwareProgrammingSagaTest
    {
        public Given_saga_When_publishing_start_message_B(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task When_publishing_start_message()
        {
            var startMessage = new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var res = await Node.NewDebugWaiter()
                                .Expect<SagaCreatedEvent<SoftwareProgrammingState>>()
                                .Create()
                                .SendToSagas(startMessage);

            var sagaId = res.Message<SagaCreatedEvent<SoftwareProgrammingState>>().State.Id;
            var state = await this.LoadSaga<SoftwareProgrammingState>(sagaId);
            // Saga_has_correct_data()
            Assert.Equal(startMessage.SofaId, state.SofaId);
            //Saga_has_correct_state()
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), state.CurrentStateName);
        }
    }
}