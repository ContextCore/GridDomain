using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Sagas.SagaRecycling.Saga
{
    public class StartEvent : DomainEvent
    {
        public StartEvent(Guid sourceId) : base(sourceId)
        {
        }
    }
}