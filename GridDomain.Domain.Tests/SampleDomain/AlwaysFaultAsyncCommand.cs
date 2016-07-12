using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.SampleDomain
{
    public class AlwaysFaultAsyncCommand : Command
    {
        public AlwaysFaultAsyncCommand(Guid aggregateId, TimeSpan? sleepTime=null)
        {
            AggregateId = aggregateId;
            SleepTime = sleepTime;
        }

        public Guid AggregateId { get; }
        public TimeSpan? SleepTime { get; set; }
    }
}