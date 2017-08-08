using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.BalloonDomain.Commands
{
    public class PlanTitleWriteAndBlowCommand : Command
    {
        public PlanTitleWriteAndBlowCommand(int parameter,
                                             Guid aggregateId,
                                             TimeSpan? sleepTime = null) : base(aggregateId)
        {
            Parameter = parameter;
            SleepTime = sleepTime ?? TimeSpan.FromSeconds(1);
        }

        public TimeSpan SleepTime { get; }
        public int Parameter { get; }
    }
}