using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.FutureEvents.Infrastructure
{
    public class ScheduleErrorInFutureCommand : Command
    {
        public ScheduleErrorInFutureCommand(DateTime raiseTime, Guid aggregateId, string value, int succedOnRetryNum):base(aggregateId)
        {
            RaiseTime = raiseTime;
            Value = value;
            SuccedOnRetryNum = succedOnRetryNum;
        }
        public DateTime RaiseTime { get; }
        public string Value { get; }
        public int SuccedOnRetryNum { get;}
    }
}