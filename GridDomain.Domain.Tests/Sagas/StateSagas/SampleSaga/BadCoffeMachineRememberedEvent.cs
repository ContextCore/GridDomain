using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga
{
    public class BadCoffeMachineRememberedEvent : DomainEvent
    {
        public Guid CoffeMachineId { get; }

        public BadCoffeMachineRememberedEvent(Guid sagaId, Guid coffeMachineId) : base(sagaId)
        {
            CoffeMachineId = coffeMachineId;
        }
    }
}