using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.Sagas.SagaRecycling.Saga
{
    public class FinishedEvent : DomainEvent
    {
        public FinishedEvent(Guid sourceId) : base(sourceId)
        {
        }
    }
}