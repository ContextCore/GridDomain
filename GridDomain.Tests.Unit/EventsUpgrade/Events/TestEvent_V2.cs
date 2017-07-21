using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.EventsUpgrade.Events
{
    public class TestEvent_V2 : DomainEvent
    {
        public TestEvent_V2(Guid sourceId, DateTime? createdTime = null, Guid processId = new Guid())
            : base(sourceId, processId: processId, createdTime: createdTime) {}

        public TestEvent_V2() : this(Guid.Empty) {}

        public int Field3 { get; set; }
    }
}