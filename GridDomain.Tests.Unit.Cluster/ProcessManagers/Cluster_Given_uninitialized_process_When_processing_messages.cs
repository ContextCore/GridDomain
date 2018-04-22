using System;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.ProcessManagers
{
    public class Cluster_Given_uninitialized_process_When_processing_messages : NodeTestKit
    {
        public Cluster_Given_uninitialized_process_When_processing_messages(ITestOutputHelper helper) :
            base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) {}

        [Fact]
        public async Task Process_state_should_not_be_changed()
        {
            var coffeMadeEvent = new CoffeMadeEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, Guid.NewGuid().ToString());
            
            Node.SendToProcessManager<SoftwareProgrammingState>(coffeMadeEvent).Wait(TimeSpan.FromMilliseconds(200));
            
            // await Task.Delay(200);
            var processStateAggregate = await Node.LoadProcess<SoftwareProgrammingState>(coffeMadeEvent.ProcessId);
            Assert.Null(processStateAggregate);
        }
    }
}