using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.EventsUpgrade.Events
{
    public class TestEvent_V2 : DomainEvent
    {
        public TestEvent_V2(string sourceId, DateTime? createdTime = null, string processId = null)
            : base(sourceId, processId: processId, createdTime: createdTime) {}

        public TestEvent_V2() : this(null) {}

        public int Field3 { get; set; }
    }
}