using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.BalloonDomain.Commands
{
    public class PlanTitleChangeCommand : Command
    {
        public PlanTitleChangeCommand(int parameter, Guid aggregateId, Guid processId = default(Guid), TimeSpan? sleepTime = null)
            : base(Guid.NewGuid(), aggregateId, processId)
        {
            Parameter = parameter;
            SleepTime = sleepTime ?? TimeSpan.FromSeconds(1);
        }

        public TimeSpan SleepTime { get; }
        public int Parameter { get; }
    }
}