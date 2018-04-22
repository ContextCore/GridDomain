using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{

    public class Given_process_When_handling_command_faults : NodeTestKit
    {
        protected Given_process_When_handling_command_faults(NodeTestFixture fixture) : base(fixture){}
        public Given_process_When_handling_command_faults(ITestOutputHelper output) :
            this(new SoftwareProgrammingProcessManagerFixture(output)) {}

        [Fact]
        public async Task When_process_produce_command_and_waiting_for_it_fault()
        {

            var givenProcessStateAggregate = new ProcessStateAggregate<SoftwareProgrammingState>(new SoftwareProgrammingState(Guid.NewGuid().ToString(), 
                                                                                            nameof(SoftwareProgrammingProcess.MakingCoffee))
                                                                                          {
                                                                                              PersonId = Guid.NewGuid().ToString()
                                                                                          });

            await Node.SaveToJournal(givenProcessStateAggregate);

            var coffeMakeFailedEvent = new CoffeMakeFailedEvent(Guid.NewGuid().ToString(),
                                                                givenProcessStateAggregate.State.PersonId,
                                                                BusinessDateTime.UtcNow,
                                                                givenProcessStateAggregate.Id);

            await Node.NewTestWaiter()
                      .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>(m => m.State.CurrentStateName == nameof(SoftwareProgrammingProcess.Coding))
                      .Create()
                      .SendToProcessManagers(coffeMakeFailedEvent, MessageMetadata.New(coffeMakeFailedEvent.SourceId, null, null));

            var processStateAggregate = await Node.LoadProcess<SoftwareProgrammingState>(givenProcessStateAggregate.Id);
            //Process_should_be_in_correct_state_after_fault_handling()
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), processStateAggregate.CurrentStateName);
            //Process_state_should_contain_data_from_fault_message()
            Assert.Equal(coffeMakeFailedEvent.ForPersonId, processStateAggregate.BadSleepPersonId);
        }
    }
}