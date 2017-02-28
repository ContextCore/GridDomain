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

        public async Task ScheduleInFuture(DateTime raiseTime, string testValue)
        {
            await Emit(new TestDomainEvent(testValue, Id), raiseTime);
        }

        public async Task ScheduleErrorInFuture(DateTime raiseTime, string testValue, int succedOnRetryNum)
        {
            if (RetriesToSucceed == 0) await Emit(new TestDomainEvent(testValue, Id), raiseTime);
            else
            { await Emit(new TestErrorDomainEvent(testValue, Id, succedOnRetryNum), raiseTime); }
        }

        public async Task CancelFutureEvents(string likeValue)
        {
           await CancelScheduledEvents<TestDomainEvent>(e => e.Value.Contains(likeValue));
        }

        private void Apply(TestDomainEvent e)
        {
            Value = e.Value;
            ProcessedTime = DateTime.Now;
        }

        private void Apply(TestErrorDomainEvent e)
        {
            if (RetriesToSucceed == null) RetriesToSucceed = e.SuccedOnRetryNum;

            if (RetriesToSucceed == 0)
            {
                RetriesToSucceed = e.SuccedOnRetryNum;
                return;
            }

            RetriesToSucceed --;
            throw new TestScheduledException(RetriesToSucceed.Value + 1);
        }
    }
}