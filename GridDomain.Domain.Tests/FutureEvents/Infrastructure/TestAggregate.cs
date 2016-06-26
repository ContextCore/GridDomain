using System;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.Node.FutureEvents;
using GridDomain.Tests.DependencyInjection;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    public class TestAggregate : FutureEventsAggregate
    {
        private TestAggregate(Guid id):base(id)
        {
        }

        public void RaiseFutureEvent(DateTime raiseTime)
        {
            RaiseEvent(new FutureDomainEvent(Id, raiseTime,new TestDomainEvent("tesst value",Id)));
        }

        private void Apply(TestDomainEvent e)
        {
            Value = e.Value;
            ProcessedTime = DateTime.Now;
        }
        public DateTime ProcessedTime { get; set; }

        public string Value;
    }
}