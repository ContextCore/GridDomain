using System;
using System.Threading.Tasks;
using GridDomain.Scheduling;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure
{
    public class TestFutureEventsAggregate : FutureEventsAggregate
    {
        public string Value;

        private TestFutureEventsAggregate(Guid id) : base(id) {}

        public TestFutureEventsAggregate(Guid id, string initialValue = "") : this(id)
        {
            Produce(new ValueChangedSuccessfullyEvent(initialValue, 1, Id));
        }

        public int? RetriesToSucceed { get; private set; }
        public DateTime ProcessedTime { get; private set; }

        public void ScheduleInFuture(DateTime raiseTime, string testValue)
        {
            Produce(new ValueChangedSuccessfullyEvent(testValue, 1, Id), raiseTime);
        }

        public async Task ScheduleErrorInFuture(DateTime raiseTime, string testValue, int succedOnRetryNum)
        {
            if (RetriesToSucceed == 0 || RetriesToSucceed == null)
            {
                Produce(new ValueChangedSuccessfullyEvent(testValue, succedOnRetryNum, Id), raiseTime);
                return;
            }

            await Emit(new RetriesToSucceedDecreasedEvent(testValue, Id, succedOnRetryNum), raiseTime);

            throw new TestScheduledException(RetriesToSucceed.Value + 1);
        }

        public void CancelFutureEvents(string likeValue)
        {
            CancelScheduledEvents<ValueChangedSuccessfullyEvent>(e => e.Value.Contains(likeValue));
        }

        private void Apply(ValueChangedSuccessfullyEvent e)
        {
            Value = e.Value;
            ProcessedTime = DateTime.Now;
            RetriesToSucceed = e.RetriesToSucceed;
        }

        private void Apply(RetriesToSucceedDecreasedEvent e)
        {
            RetriesToSucceed--;
        }
    }
}