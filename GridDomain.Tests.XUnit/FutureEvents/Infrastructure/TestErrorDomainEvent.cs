using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.FutureEvents.Infrastructure
{
    public class TestErrorDomainEvent : DomainEvent
    {
        public TestErrorDomainEvent(string value, Guid id, int succedOnRetryNum) : base(id)
        {
            Value = value;
            SuccedOnRetryNum = succedOnRetryNum;
        }

        public string Value { get; }
        public int SuccedOnRetryNum { get; }
    }
}