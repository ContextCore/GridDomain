using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestProcessStartMessage : DomainEvent
    {
        public TestProcessStartMessage(Guid sourceId, DateTime? createdTime = null, Guid processId = new Guid())
            : base(sourceId, processId: processId, createdTime: createdTime) {}
    }
}