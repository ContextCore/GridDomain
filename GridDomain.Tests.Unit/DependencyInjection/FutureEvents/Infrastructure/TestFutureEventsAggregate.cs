using System;
using GridDomain.EventSourcing;
using GridDomain.Scheduling;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure
{
    public class TestFutureEventsAggregate : FutureEventsAggregate
    {
        public string Value;

        private TestFutureEventsAggregate(Guid id) : base(id) {}

        public TestFutureEventsAggregate(Guid id, string initialValue = "") : this(id)
        {
            Produce(new TestDomainEvent(initialValue, Id));
        }

        public int? RetriesToSucceed { get; private set; }
        public DateTime ProcessedTime { get; private set; }

        public void ScheduleInFuture(DateTime raiseTime, string testValue)
        {
            Emit(new TestDomainEvent(testValue, Id), raiseTime);
        }

        public void ScheduleErrorInFuture(DateTime raiseTime, string testValue, int succedOnRetryNum)
        {
            if (RetriesToSucceed == null)
                RetriesToSucceed = succedOnRetryNum;

            if (RetriesToSucceed == 0)
            {
                Emit(new TestDomainEvent(testValue, Id), raiseTime);
                return;
            }

            Emit(new TestErrorDomainEvent(testValue, Id, succedOnRetryNum), raiseTime);
        }

        public void CancelFutureEvents(string likeValue)
        {
            CancelScheduledEvents<TestDomainEvent>(e => e.Value.Contains(likeValue));
        }

        private void Apply(TestDomainEvent e)
        {
            Value = e.Value;
            ProcessedTime = DateTime.Now;
        }

        private void Apply(TestErrorDomainEvent e)
        {
            if (!RetriesToSucceed.HasValue)
            {
                RetriesToSucceed = e.SuccedOnRetryNum;
                return;
            }

            if (RetriesToSucceed == 0)
            {
                RetriesToSucceed = e.SuccedOnRetryNum;
                return;
            }

            RetriesToSucceed--;
            throw new TestScheduledException(RetriesToSucceed.Value + 1);
        }
    }
}