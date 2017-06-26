using System;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestEvent : DomainEvent
    {
        public TestEvent(Guid id) : base(id, sagaId: id, createdTime: BusinessDateTime.UtcNow) {}
    }
}