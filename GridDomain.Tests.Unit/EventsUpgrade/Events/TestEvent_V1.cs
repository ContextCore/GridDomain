using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.EventsUpgrade.Events
{
    public class TestEvent_V1 : DomainEvent
    {
        public TestEvent_V1(string sourceId, DateTime? createdTime = null, string processId = null)
            : base(sourceId, processId: processId, createdTime: createdTime) {}

        public TestEvent_V1() : this(null) {}

        public int Field2 { get; set; }
    }
}