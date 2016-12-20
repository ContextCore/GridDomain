using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga
{
    public class BadCoffeMachineRememberedEvent : DomainEvent
    {
        public Guid CoffeMachineId { get; }

        public BadCoffeMachineRememberedEvent(Guid sourceId, Guid coffeMachineId) : base(sourceId)
        {
            CoffeMachineId = coffeMachineId;
        }
    }
}