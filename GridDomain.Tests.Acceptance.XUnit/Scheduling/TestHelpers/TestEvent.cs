using System;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class TestEvent : DomainEvent
    {
        public TestEvent(Guid id) : base(id, BusinessDateTime.UtcNow, id)
        {
        }
    }
}