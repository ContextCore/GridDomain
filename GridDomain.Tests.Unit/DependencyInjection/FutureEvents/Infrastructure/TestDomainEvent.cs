using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure
{
    public class TestDomainEvent : DomainEvent
    {
        public string Value;

        public TestDomainEvent(string value,
                               Guid sourceId,
                               Guid id = default(Guid),
                               DateTime? createdTime = default(DateTime?),
                               Guid sagaId = default(Guid)) : base(sourceId, sagaId: sagaId, createdTime: createdTime)
        {
            Value = value;
            Id = id;
        }

        public new Guid Id { get; }
    }
}