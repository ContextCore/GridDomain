using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestProcessStartMessage : DomainEvent
    {
        public TestProcessStartMessage(string sourceId, DateTime? createdTime = null, string processId = null)
            : base(sourceId, processId: processId, createdTime: createdTime) {}
    }
}