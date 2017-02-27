using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.SampleDomain.Commands
{
    public class AsyncMethodCommand : Command
    {
        public AsyncMethodCommand(int parameter, Guid aggregateId, Guid sagaId = default(Guid),TimeSpan? sleepTime = null)
            :base(Guid.NewGuid(), aggregateId, sagaId)
        {
            Parameter = parameter;
            SleepTime = sleepTime??TimeSpan.FromSeconds(1);
        }

        public TimeSpan SleepTime { get; }
        public int Parameter { get; }
    }
}