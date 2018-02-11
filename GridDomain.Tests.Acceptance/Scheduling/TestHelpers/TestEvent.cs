using System;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestEvent : DomainEvent
    {
        public TestEvent(string id) : base(id, processId: id, createdTime: BusinessDateTime.UtcNow) {}
    }
}