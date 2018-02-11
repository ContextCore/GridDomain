using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure
{
    public class TestDomainEvent : DomainEvent
    {
        public string Value;

        public TestDomainEvent(string value,
                               string sourceId,
                               DateTime? createdTime = default(DateTime?),
                               string processId = null) : base(sourceId, processId: processId, createdTime: createdTime)
        {
            Value = value;
        }
    }
}