using System;
using System.Threading.Tasks;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Processes.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class Given_process_When_publishing_any_of_start_messages : SoftwareProgrammingProcessTest
    {
        public Given_process_When_publishing_any_of_start_messages(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task When_publishing_start_message()
        {
            var res = await
                Node.NewDebugWaiter()
                    .Expect<ProcessManagerCreated<SoftwareProgrammingState>>()
                    .Create()
                    .SendToProcessManagers(new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid()));

            var processCreatedEvent = res.Message<ProcessManagerCreated<SoftwareProgrammingState>>();

            var state = await this.LoadProcessByActor<SoftwareProgrammingState>(processCreatedEvent.State.Id);
            //Process_data_is_not_null()
            Assert.NotNull(state);
            //Process_has_correct_id()
            Assert.Equal(processCreatedEvent.SourceId, state.Id);
        }
    }
}