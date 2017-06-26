using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.EventsUpgrade.Events
{
    public class TestEvent_V1 : DomainEvent
    {
        public TestEvent_V1(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid())
            : base(sourceId, sagaId: sagaId, createdTime: createdTime) {}

        public TestEvent_V1() : this(Guid.Empty) {}

        public int Field2 { get; set; }
    }
}