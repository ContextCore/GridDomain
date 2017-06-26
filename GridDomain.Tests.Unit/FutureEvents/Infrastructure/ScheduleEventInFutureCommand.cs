using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.FutureEvents.Infrastructure
{
    public class ScheduleEventInFutureCommand : Command
    {
        public ScheduleEventInFutureCommand(DateTime raiseTime, Guid aggregateId, string value) : base(aggregateId)
        {
            RaiseTime = raiseTime;
            Value = value;
        }

        public DateTime RaiseTime { get; }
        public string Value { get; }
    }
}