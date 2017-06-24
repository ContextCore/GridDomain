using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Common;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.Transitions
{
    public class Given_created_event_when_hydrating
    {
        [Fact]
        public void Given_created_event()
        {
            var sagaId = Guid.NewGuid();
            var softwareProgrammingState = new SoftwareProgrammingState(sagaId, nameof(SoftwareProgrammingProcess.Sleeping));
            var aggregate = Aggregate.Empty<SagaStateAggregate<SoftwareProgrammingState>>(sagaId);
            aggregate.ApplyEvents(new SagaCreated<SoftwareProgrammingState>(softwareProgrammingState, sagaId));

            //Then_State_is_taken_from_event()
            Assert.Equal(softwareProgrammingState, aggregate.State);
            //Then_Id_is_taken_from_event()
            Assert.Equal(sagaId, aggregate.Id);
        }
    }
}