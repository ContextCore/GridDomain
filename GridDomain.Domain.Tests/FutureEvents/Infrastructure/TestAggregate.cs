using System;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.Node.FutureEvents;
using GridDomain.Tests.DependencyInjection;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    public class TestAggregate : Aggregate
    {
        private TestAggregate(Guid id):base(id)
        {
        }

        public void ScheduleInFuture(DateTime raiseTime)
        {
            RaiseEvent(raiseTime, new TestDomainEvent("test value",Id));
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