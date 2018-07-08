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
            
            var startMessageA = new GotTiredEvent("man_1", "sofa_1", "machine_1");

            var resA = await Node.PrepareForProcessManager(startMessageA)
                                 .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                                 .Send();

            var stateA = resA.Received.State;

            Assert.Equal(startMessageA.SourceId, stateA.PersonId);

            var secondStartMessageB = new SleptWellEvent("man_2", "sofa_2");

            var resB = await Node.PrepareForProcessManager(secondStartMessageB)
                                 .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                                 .Send();

            var stateB = resB.Received.State;

            Assert.NotEqual(stateA.Id, stateB.Id);
            Assert.Equal(secondStartMessageB.SofaId, stateB.SofaId);
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), stateB.CurrentStateName);
         
        }
    }
}