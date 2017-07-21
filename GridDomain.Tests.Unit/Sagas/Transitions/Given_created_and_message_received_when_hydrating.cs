using System;
using GridDomain.Processes.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.Unit.Sagas.Transitions
{
    public class Given_created_and_message_received_when_hydrating 
    {
        [Fact]
        public void Given_created_and_message_received_and_transitioned_event()
        {
            var sagaId = Guid.NewGuid();
            var softwareProgrammingState = new SoftwareProgrammingState(sagaId, nameof(SoftwareProgrammingProcess.Sleeping));

            var aggregate = EventSourcing.Aggregate.Empty<ProcessStateAggregate<SoftwareProgrammingState>>(sagaId);
            aggregate.ApplyEvents(new SagaCreated<SoftwareProgrammingState>(softwareProgrammingState, sagaId),
                                  new SagaReceivedMessage<SoftwareProgrammingState>(sagaId,
                                                                                    softwareProgrammingState,
                                                                                    new GotTiredEvent(Guid.NewGuid())));
            Assert.Equal(softwareProgrammingState, aggregate.State);
        }
    }
}