using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure
{
    public class ScheduleEventInFutureCommand : Command<TestFutureEventsAggregate>
    {
        public ScheduleEventInFutureCommand(DateTime raiseTime, string aggregateId, string value) : base(aggregateId)
        {
            RaiseTime = raiseTime;
            Value = value;
        }

        public DateTime RaiseTime { get; }
        public string Value { get; }
    }
}