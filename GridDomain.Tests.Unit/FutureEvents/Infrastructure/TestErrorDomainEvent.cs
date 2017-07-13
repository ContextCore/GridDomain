using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure
{
    public class RetriesToSucceedDecreasedEvent : DomainEvent
    {
        public RetriesToSucceedDecreasedEvent(string value, Guid sourceId, int succedOnRetryNum) : base(sourceId)
        {
            Value = value;
            SuccedOnRetryNum = succedOnRetryNum;
        }

        public string Value { get; }
        public int SuccedOnRetryNum { get; }
    }
}