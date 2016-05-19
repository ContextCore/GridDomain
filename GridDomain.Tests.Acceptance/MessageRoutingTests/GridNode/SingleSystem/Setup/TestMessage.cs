using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup
{
    public class TestMessage : ICommand
    {
        public Guid CorrelationId { get; set; }
        public Guid ProcessedBy { get; }
        public Guid Id { get; } = Guid.NewGuid();
        public Guid SagaId { get; }
        public long HandlerHashCode { get; set; }

        public int HandleOrder { get; set; }
        public int ExecuteOrder { get; set; }
    }
}