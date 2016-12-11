using System;
using CommonDomain.Core;
using GridDomain.EventSourcing.Sagas.FutureEvents;
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
        public void ScheduleErrorInFuture(DateTime raiseTime, string testValue, int succedOnRetryNum)
        {
            if (RetriesToSucceed == 0)
                RaiseEvent(raiseTime, new TestDomainEvent(testValue, Id));
            else
            {
                RaiseEvent(raiseTime, new TestErrorDomainEvent(testValue, Id, succedOnRetryNum));
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