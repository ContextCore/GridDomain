using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure
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
            RaiseEvent(new TestDomainEvent(testValue,Id), raiseTime);
        }
        public void ScheduleErrorInFuture(DateTime raiseTime, string testValue, int succedOnRetryNum)
        {
            if (RetriesToSucceed == 0)
                RaiseEvent(new TestDomainEvent(testValue, Id), raiseTime);
            else
            {
                RaiseEvent(new TestErrorDomainEvent(testValue, Id, succedOnRetryNum), raiseTime);
            }
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

        private void Apply(TestErrorDomainEvent e)
        {
            if (RetriesToSucceed == null)
                RetriesToSucceed = e.SuccedOnRetryNum;

            if (RetriesToSucceed == 0)
            {
                RetriesToSucceed = e.SuccedOnRetryNum;
                return;
            }

            RetriesToSucceed --;
            throw new TestScheduledException(RetriesToSucceed.Value + 1);
        }

        public int? RetriesToSucceed { get; private set; }
        public DateTime ProcessedTime { get; private set; }

        public string Value;
    }
}