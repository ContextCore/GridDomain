using System;
using System.Threading.Tasks;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class Given_process_When_publishing_several_start_messages : NodeTestKit
    {
        protected Given_process_When_publishing_several_start_messages(NodeTestFixture fixture):base(fixture){} 
        
        public Given_process_When_publishing_several_start_messages(ITestOutputHelper helper): 
            this(new SoftwareProgrammingProcessManagerFixture(helper).IgnorePipeCommands()) {}

        [Fact]
        public async Task Then_separate_process_startes_on_each_message()
        {
            
            var startMessageA = new GotTiredEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var resA = await Node.NewLocalDebugWaiter()
                                 .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                                 .Create()
                                 .SendToProcessManagers(startMessageA);

            var stateA = resA.Message<ProcessReceivedMessage<SoftwareProgrammingState>>().State;

            var secondStartMessageB = new SleptWellEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var resB = await Node.NewLocalDebugWaiter()
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