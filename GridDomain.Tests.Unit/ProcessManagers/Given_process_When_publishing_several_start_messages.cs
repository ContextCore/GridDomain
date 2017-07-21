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
    public class Given_process_When_publishing_several_start_messages : SoftwareProgrammingProcessTest
    {
        public Given_process_When_publishing_several_start_messages(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task Then_separate_process_startes_on_each_message()
        {
            var startMessageA = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var resA = await Node.NewDebugWaiter()
                                 .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                                 .Create()
                                 .SendToProcessManagers(startMessageA);

            var stateA = resA.Message<ProcessReceivedMessage<SoftwareProgrammingState>>().State;

            //will reach same process as already created and will produce a new one
            var secondStartMessageB = new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), stateA.Id);

            var resB = await Node.NewDebugWaiter()
                                 .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                                 .Create()
                                 .SendToProcessManagers(secondStartMessageB);

            var stateB = resB.Message<ProcessReceivedMessage<SoftwareProgrammingState>>().State;

            Assert.Equal(secondStartMessageB.SofaId, stateB.SofaId);
            //Process_has_correct_state()
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), stateB.CurrentStateName);
            Assert.NotEqual(stateA.Id, stateB.Id);
        }
    }
}