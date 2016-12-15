using System;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Tests.DependencyInjection;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    public class TestAggregate : Aggregate
    {
        private TestAggregate(Guid id):base(id)
        {
        }
        public TestAggregate(Guid id, string initialValue =""):this(id)
        {
            Value = initialValue;
        }

        public void ScheduleInFuture(DateTime raiseTime, string testValue)
        {
            RaiseEvent(raiseTime, new TestDomainEvent(testValue,Id));
        }

        public void ScheduleErrorInFuture(DateTime raiseTime, string testValue)
        {
            RaiseEvent(raiseTime, new ErrorTestDomainEvent(testValue, Id));
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

        private void Apply(ErrorTestDomainEvent e)
        {
            throw new TestAggregateException(e.TestValue);
        }

        public DateTime ProcessedTime { get; private set; }

        public string Value;
    }

    internal class TestAggregateException : Exception
    {
        public string TestValue { get; }

        public TestAggregateException(string testValue)
        {
            TestValue = testValue;
        }
    }

    public class ErrorTestDomainEvent : DomainEvent
    {
        public string TestValue { get; }

        public ErrorTestDomainEvent(string testValue, Guid id):base(id)
        {
            TestValue = testValue;
        }
    }
}