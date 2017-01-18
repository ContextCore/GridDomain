using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup
{
    public class TestEvent : DomainEvent
    {
        public Guid CorrelationId { get; set; }
        public Guid ProcessedBy { get; }
        public long HandlerHashCode { get; set; }

        public int HandleOrder { get; set; }
        public int ExecuteOrder { get; set; }
        public Guid Id => SourceId;

        public TestEvent() : base(Guid.NewGuid())
        {
        }
    }
}