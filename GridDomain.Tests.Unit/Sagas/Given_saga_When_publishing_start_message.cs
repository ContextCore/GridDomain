using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Common;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class Given_saga_When_publishing_start_message : SoftwareProgrammingSagaTest
    {
        public Given_saga_When_publishing_start_message(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task When_publishing_start_message_A()
        {
            var startMessage = new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var state = await Node.GetTransitedState<SoftwareProgrammingState>(startMessage);
            // Saga_has_correct_data()
            Assert.Equal(startMessage.SofaId, state.SofaId);
            //Saga_has_correct_state()
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), state.CurrentStateName);
        }


        [Fact]
        public async Task When_publishing_start_message_B()
        {
            var startMessage = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var state = await Node.GetTransitedState<SoftwareProgrammingState>(startMessage);

            //Saga_has_correct_data()
            Assert.Equal(startMessage.PersonId, state.PersonId);
            //Saga_has_correct_state()
            Assert.Equal(nameof(SoftwareProgrammingProcess.MakingCoffee), state.CurrentStateName);
        }
    }
}