using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure
{
    public class TestDomainEvent : DomainEvent
    {
        public string Value;

        public TestDomainEvent(string value,
                               Guid sourceId,
                               DateTime? createdTime = default(DateTime?),
                               Guid processId = default(Guid)) : base(sourceId, processId: processId, createdTime: createdTime)
        {
            Value = value;
        }
    }
}