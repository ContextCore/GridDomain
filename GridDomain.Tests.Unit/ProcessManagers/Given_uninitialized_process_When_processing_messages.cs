using System;
using System.Threading.Tasks;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class Given_uninitialized_process_When_processing_messages : NodeTestKit
    {
        public Given_uninitialized_process_When_processing_messages(ITestOutputHelper helper) : this(new SoftwareProgrammingProcessManagerFixture(helper)) {}
        protected Given_uninitialized_process_When_processing_messages(NodeTestFixture fixture) : base(fixture) {}

        [Fact]
        public async Task Process_state_should_not_be_changed()
        {
            var coffeMadeEvent = new CoffeMadeEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, Guid.NewGuid().ToString());
            
            Node.PrepareForProcessManager(coffeMadeEvent).Expect<SoftwareProgrammingState>().Send().Wait(TimeSpan.FromMilliseconds(200));
            
            var processStateAggregate = await Node.LoadProcess<SoftwareProgrammingState>(coffeMadeEvent.ProcessId);
            Assert.Null(processStateAggregate);
        }
    }
}