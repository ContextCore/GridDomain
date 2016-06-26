using System;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.Tests.DependencyInjection;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    public class TestAggregate : AggregateBase
    {
        private TestAggregate(Guid id)
        {
            Id = id;
        }

        public void RaiseFutureEvent(DateTime raiseTime)
        {
            RaiseEvent(new FutureDomainEvent(Id, raiseTime,new TestDomainEvent("tesst value",Id)));
        }

        private void Apply(TestDomainEvent e)
        {
            Value = e.Value;
            e.processedTime = DateTime.Now;
        }

        public string Value;
    }
    public class TestDomainEvent : DomainEvent
    {
        public string Value;
        public TestDomainEvent(string value, Guid sourceId, DateTime? createdTime = default(DateTime?), Guid sagaId = default(Guid)) : base(sourceId, createdTime, sagaId)
        {
            Value = value;

        }

        public DateTime processedTime { get; set; }
    }


}