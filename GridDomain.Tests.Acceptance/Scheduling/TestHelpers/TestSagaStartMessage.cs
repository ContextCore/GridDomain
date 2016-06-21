using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestSagaStartMessage : DomainEvent
    {
        public TestSagaStartMessage(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
        }
    }
}