using System;
using GridDomain.EventSourcing;
using GridDomain.Processes.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using Xunit;

namespace GridDomain.Tests.Unit.ProcessManagers.Transitions
{
    public class Given_created_event_when_hydrating
    {
        [Fact]
        public void Given_created_event()
        {
            var processId = Guid.NewGuid();
            var softwareProgrammingState = new SoftwareProgrammingState(processId, nameof(SoftwareProgrammingProcess.Sleeping));
            var aggregate = Aggregate.Empty<ProcessStateAggregate<SoftwareProgrammingState>>(processId);
            aggregate.ApplyEvents(new ProcessManagerCreated<SoftwareProgrammingState>(softwareProgrammingState, processId));

            //Then_State_is_taken_from_event()
            Assert.Equal(softwareProgrammingState, aggregate.State);
            //Then_Id_is_taken_from_event()
            Assert.Equal(processId, aggregate.Id);
        }
    }
}