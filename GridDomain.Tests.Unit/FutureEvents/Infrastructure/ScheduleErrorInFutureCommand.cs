using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure
{
    public class ScheduleErrorInFutureCommand : Command
    {
        public ScheduleErrorInFutureCommand(DateTime raiseTime, Guid aggregateId, string value, int succedOnRetryNum)
        {
            RaiseTime = raiseTime;
            AggregateId = aggregateId;
            Value = value;
            SuccedOnRetryNum = succedOnRetryNum;
        }

        public Guid AggregateId { get; }
        public DateTime RaiseTime { get; }
        public string Value { get; }
        public int SuccedOnRetryNum { get;}
    }
}