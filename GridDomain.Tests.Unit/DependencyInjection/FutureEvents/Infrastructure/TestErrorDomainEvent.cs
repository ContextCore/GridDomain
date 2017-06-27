using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure
{
    public class TestErrorDomainEvent : DomainEvent
    {
        public TestErrorDomainEvent(string value, Guid sourceId, int succedOnRetryNum) : base(sourceId)
        {
            Value = value;
            SuccedOnRetryNum = succedOnRetryNum;
        }

        public string Value { get; }
        public int SuccedOnRetryNum { get; }
    }
}