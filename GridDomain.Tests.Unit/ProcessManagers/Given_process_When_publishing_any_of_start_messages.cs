using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class Given_process_When_publishing_any_of_start_messages : NodeTestKit
    {
        public Given_process_When_publishing_any_of_start_messages(ITestOutputHelper helper) : this(new SoftwareProgrammingProcessManagerFixture(helper)) {}
        protected Given_process_When_publishing_any_of_start_messages(NodeTestFixture helper) : base(helper) {}

        [Fact]
        public async Task When_publishing_start_message()
        {
            var evt = new SleptWellEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var metadata = MessageMetadata.New(evt.Id, Guid.NewGuid().ToString(), null);
            var res = await
                Node.NewTestWaiter()
                    .Expect<ProcessManagerCreated<SoftwareProgrammingState>>()
                    .Create()
                    .SendToProcessManagers(evt, metadata);

            var processCreatedEvent = res.Message<ProcessManagerCreated<SoftwareProgrammingState>>();

            var state = await Node.LoadProcess<SoftwareProgrammingState>(processCreatedEvent.State.Id);
            //Process_data_is_not_null()
            Assert.NotNull(state);
            //Process_has_correct_id()
            Assert.Equal(processCreatedEvent.SourceId, state.Id);
        }
    }
}