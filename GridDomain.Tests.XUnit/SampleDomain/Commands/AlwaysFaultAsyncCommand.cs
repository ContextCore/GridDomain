using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.SampleDomain.Commands
{
    public class AlwaysFaultAsyncCommand : Command
    {
        public AlwaysFaultAsyncCommand(Guid aggregateId, TimeSpan? sleepTime = null) : base(aggregateId)
        {
            SleepTime = sleepTime ?? TimeSpan.FromMilliseconds(500);
        }

        public TimeSpan SleepTime { get; }
    }
}