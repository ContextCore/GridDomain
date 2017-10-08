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
        public Given_process_When_handling_command_faults(ITestOutputHelper output) :
            base(output, new SoftwareProgrammingProcessManagerFixture()) {}

        [Fact]
        public async Task When_process_produce_command_and_waiting_for_it_fault()
        {

            var givenProcessStateAggregate = new ProcessStateAggregate<SoftwareProgrammingState>(new SoftwareProgrammingState(Guid.NewGuid(), 
                                                                                            nameof(SoftwareProgrammingProcess.MakingCoffee))
                                                                                          {
                                                                                              PersonId = Guid.NewGuid()
                                                                                          });

            await Node.SaveToJournal(givenProcessStateAggregate);

            var coffeMakeFailedEvent = new CoffeMakeFailedEvent(Guid.NewGuid(),
                                                                givenProcessStateAggregate.State.PersonId,
                                                                BusinessDateTime.UtcNow,
                                                                givenProcessStateAggregate.Id);

            await Node.NewDebugWaiter()
                      .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>(m => m.State.CurrentStateName == nameof(SoftwareProgrammingProcess.Coding))
                      .Create()
                      .SendToProcessManagers(coffeMakeFailedEvent, new MessageMetadata(coffeMakeFailedEvent.SourceId));

            var processStateAggregate = await this.LoadProcessByActor<SoftwareProgrammingState>(givenProcessStateAggregate.Id);
            //Process_should_be_in_correct_state_after_fault_handling()
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), processStateAggregate.CurrentStateName);
            //Process_state_should_contain_data_from_fault_message()
            Assert.Equal(coffeMakeFailedEvent.ForPersonId, processStateAggregate.BadSleepPersonId);
        }
    }
}