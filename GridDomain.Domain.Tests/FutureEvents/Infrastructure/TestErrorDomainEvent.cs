using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    public class TestErrorDomainEvent : DomainEvent
    {
        public string Value { get; }
        public int SuccedOnRetryNum { get; }

        public TestErrorDomainEvent(string value, Guid id, int succedOnRetryNum):base(id)
        {
            this.Value = value;
            SuccedOnRetryNum = succedOnRetryNum;
        }
    }
}