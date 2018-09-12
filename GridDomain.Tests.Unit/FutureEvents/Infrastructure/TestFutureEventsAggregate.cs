using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Scheduling;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure
{
    public class TestFutureEventsAggregate : FutureEventsAggregate
    {
        public string Value;

        private TestFutureEventsAggregate(string id) : base(id) {}

        public TestFutureEventsAggregate(string id, string initialValue = "") : this(id)
        {
            Execute<ScheduleEventInFutureCommand>(c => ScheduleInFuture(c.RaiseTime, c.Value));
            Execute<CancelFutureEventCommand>( c => CancelFutureEvents(c.Value));
            Execute<BoomNowCommand>(c => Boom());
            Execute<PlanBoomCommand>(c => PlanBoom(c.BoomTime));
            
            Emit(new ValueChangedSuccessfullyEvent(initialValue, 1, Id));
        }

        public int? RetriesToSucceed { get; private set; }
        public DateTime ProcessedTime { get; private set; }

        public void ScheduleInFuture(DateTime raiseTime, string testValue)
        {
            Emit(new ValueChangedSuccessfullyEvent(testValue, 1, Id),raiseTime,null);
        }

        public void Boom()
        {
            throw new TestScheduledException(0);
        }

        public void PlanBoom(DateTime raiseTime)
        {
            Emit(new BoomDomainEvent(Id),raiseTime,null);
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

        private void Apply(BoomDomainEvent e)
        {
            throw new TestScheduledException(0);
        }
    }
}