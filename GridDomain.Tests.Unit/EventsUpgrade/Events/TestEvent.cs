using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.EventsUpgrade.Events
{
    public class TestEvent : DomainEvent
    {
        public TestEvent(string sourceId, DateTime? createdTime = null, string processId = null)
            : base(sourceId, processId: processId, createdTime: createdTime) {}

        public TestEvent() : this(null) {}

        public int Field { get; set; }
    }
}