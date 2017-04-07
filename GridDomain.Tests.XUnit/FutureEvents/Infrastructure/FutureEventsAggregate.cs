using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.FutureEvents.Infrastructure
{
    public class FutureEventsAggregate : Aggregate
    {
        public string Value;

        private FutureEventsAggregate(Guid id) : base(id) {}

        public FutureEventsAggregate(Guid id, string initialValue = "") : this(id)
        {
            RaiseEvent(new TestDomainEvent(initialValue, Id));
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
            var testErrorDomainEvent = new TestErrorDomainEvent(testValue, Id, succedOnRetryNum);

            Emit(testErrorDomainEvent, () => { throw new TestScheduledException(RetriesToSucceed.Value + 1); }, raiseTime);
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
            if (RetriesToSucceed == 0)
            {
                RetriesToSucceed = e.SuccedOnRetryNum;
                return;
            }

            RetriesToSucceed--;
            //throw new TestScheduledException(RetriesToSucceed.Value + 1);
        }
    }
}