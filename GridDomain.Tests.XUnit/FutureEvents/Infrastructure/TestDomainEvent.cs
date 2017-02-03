using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.FutureEvents.Infrastructure
{
    public class TestDomainEvent : DomainEvent
    {
        public Guid Id { get; }
        public string Value;
        public TestDomainEvent(string value, Guid sourceId, Guid id = default(Guid), DateTime? createdTime = default(DateTime?), Guid sagaId = default(Guid)) : base(sourceId, createdTime, sagaId)
        {
            Value = value;
            Id = id;
        }
    }
}