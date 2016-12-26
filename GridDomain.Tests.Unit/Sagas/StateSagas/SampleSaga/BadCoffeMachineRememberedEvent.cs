using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.Sagas.StateSagas.SampleSaga
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