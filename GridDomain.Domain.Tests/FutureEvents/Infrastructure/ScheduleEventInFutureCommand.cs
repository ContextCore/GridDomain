using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    public class ScheduleEventInFutureCommand : Command
    {
        public ScheduleEventInFutureCommand(DateTime raiseTime, Guid aggregateId, string value)
        {
            RaiseTime = raiseTime;
            AggregateId = aggregateId;
            Value = value;
        }

        public Guid AggregateId { get; }
        public DateTime RaiseTime { get; }
        public string Value { get; }
    }
}