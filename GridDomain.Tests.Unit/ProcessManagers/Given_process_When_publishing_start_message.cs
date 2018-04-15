using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class Given_process_When_publishing_start_message : NodeTestKit
    {
        public Given_process_When_publishing_start_message(ITestOutputHelper helper) : this(new SoftwareProgrammingProcessManagerFixture(helper)) {}
        protected Given_process_When_publishing_start_message(NodeTestFixture fixture) : base(fixture) {}

        [Fact]
        public async Task When_publishing_start_message_A()
        {
            var startMessage = new SleptWellEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var state = await Node.GetTransitedState<SoftwareProgrammingState>(startMessage);
            // process_has_correct_data()
            Assert.Equal(startMessage.SofaId, state.SofaId);
            //process_has_correct_state()
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), state.CurrentStateName);
        }


        [Fact]
        public async Task When_publishing_start_message_B()
        {
            var startMessage = new GotTiredEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var state = await Node.GetTransitedState<SoftwareProgrammingState>(startMessage);

            //process_has_correct_data()
            Assert.Equal(startMessage.PersonId, state.PersonId);
            //process_has_correct_state()
            Assert.Equal(nameof(SoftwareProgrammingProcess.MakingCoffee), state.CurrentStateName);
        }
    }
}