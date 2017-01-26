using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.SampleDomain.Commands
{
    public class AsyncFaultWithOneEventCommand : Command
    {
        public AsyncFaultWithOneEventCommand(int parameter, Guid aggregateId, Guid sagaId = default(Guid), TimeSpan? sleepTime = null) : base(Guid.NewGuid(), sagaId)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
            SleepTime = sleepTime ?? TimeSpan.FromSeconds(1);
        }

        public Guid AggregateId { get; }
        public TimeSpan SleepTime { get; }
        public int Parameter { get; }
    }
}