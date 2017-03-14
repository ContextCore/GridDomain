using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.SampleDomain.Commands
{
    public class AsyncFaultWithOneEventCommand : Command
    {
        public AsyncFaultWithOneEventCommand(int parameter,
                                             Guid aggregateId,
                                             Guid sagaId = default(Guid),
                                             TimeSpan? sleepTime = null) : base(aggregateId, sagaId)
        {
            Parameter = parameter;
            SleepTime = sleepTime ?? TimeSpan.FromSeconds(1);
        }

        public TimeSpan SleepTime { get; }
        public int Parameter { get; }
    }
}