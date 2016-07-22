using System;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Node.FutureEvents;
using GridDomain.Tests.DependencyInjection;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    public class TestAggregate : Aggregate
    {
        private TestAggregate(Guid id):base(id)
        {
        }
        public  TestAggregate(Guid id, string initialValue =""):this(id)
        {
            Value = initialValue;
        }

        public void ScheduleInFuture(DateTime raiseTime, string testValue)
        {
            RaiseEvent(raiseTime, new TestDomainEvent(testValue,Id));
        }

        public void CancelFutureEvents(string likeValue)
        {
            base.CancelScheduledEvents<TestDomainEvent>(e => e.Value.Contains(likeValue));
        }
        private void Apply(TestDomainEvent e)
        {
            Value = e.Value;
            ProcessedTime = DateTime.Now;
        }
        public DateTime ProcessedTime { get; private set; }

        public string Value;
    }
}