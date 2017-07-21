using System;
using System.Threading.Tasks;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class Given_uninitialized_process_When_processing_messages : SoftwareProgrammingProcessTest
    {
        public Given_uninitialized_process_When_processing_messages(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task Process_state_should_not_be_changed()
        {
            var coffeMadeEvent = new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid());

            Node.Transport.Publish(coffeMadeEvent);
            await Task.Delay(200);
            var processStateAggregate =
                await this.LoadAggregateByActor<ProcessStateAggregate<SoftwareProgrammingState>>(coffeMadeEvent.ProcessId);
            Assert.Null(processStateAggregate.State);
        }
    }
}