using System;
using GridDomain.EventSourcing;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.Unit.ProcessManagers.Transitions
{
    public class Given_created_and_message_received_when_hydrating 
    {
        [Fact]
        public void Given_created_and_message_received_and_transitioned_event()
        {
            var processId = Guid.NewGuid().ToString();
            var softwareProgrammingState = new SoftwareProgrammingState(processId, nameof(SoftwareProgrammingProcess.Sleeping));

            var aggregate = AggregateFactory.BuildEmpty<ProcessStateAggregate<SoftwareProgrammingState>>(processId);
            aggregate.ApplyEvents(new ProcessManagerCreated<SoftwareProgrammingState>(softwareProgrammingState, processId),
                                  new ProcessReceivedMessage<SoftwareProgrammingState>(processId,
                                                                                    softwareProgrammingState,
                                                                                    Guid.NewGuid().ToString()));
            Assert.Equal(softwareProgrammingState, aggregate.State);
        }
    }
}