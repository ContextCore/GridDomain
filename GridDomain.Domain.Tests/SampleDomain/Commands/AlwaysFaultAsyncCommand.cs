using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.SampleDomain.Commands
{
    public class AlwaysFaultAsyncCommand : Command
    {
        public AlwaysFaultAsyncCommand(Guid aggregateId, TimeSpan? sleepTime=null)
        {
            AggregateId = aggregateId;
            SleepTime = sleepTime ?? TimeSpan.FromMilliseconds(500);
        }

        public Guid AggregateId { get; }
        public TimeSpan SleepTime { get;}
    }
}