using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure
{
    public class TestDomainEvent : DomainEvent
    {
        public string Value;
        public TestDomainEvent(string value, Guid sourceId, DateTime? createdTime = default(DateTime?), Guid sagaId = default(Guid)) : base(sourceId, createdTime, sagaId)
        {
            Value = value;

        }
    }
}