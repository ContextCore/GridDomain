using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.EventsUpgrade
{
    public class TestEvent_V1 : DomainEvent
    {
        public TestEvent_V1(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
        }
        public TestEvent_V1() : this(Guid.Empty)
        {

        }

        public int Field2 { get; set; }
    }

    public class TestEvent_V2: DomainEvent
    {
        public TestEvent_V2(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {

        }

        public TestEvent_V2(): this(Guid.Empty)
        {

        }

        public int Field3 { get; set; }
    }

    public class TestEvent_V3 : DomainEvent
    {
        public TestEvent_V3(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
        }
        public TestEvent_V3() : this(Guid.Empty)
        {

        }

        public int Field4 { get; set; }
    }
}