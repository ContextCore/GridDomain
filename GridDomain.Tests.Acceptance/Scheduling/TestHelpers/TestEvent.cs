using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestEvent : DomainEvent
    {
        public TestEvent(Guid id) : base(id,DateTime.UtcNow, id)
        {
        }
    }
}