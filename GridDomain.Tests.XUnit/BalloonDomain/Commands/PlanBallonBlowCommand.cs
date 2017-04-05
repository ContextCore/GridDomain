using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.BalloonDomain.Commands
{
    public class PlanBallonBlowCommand : Command
    {
        public PlanBallonBlowCommand(Guid aggregateId, TimeSpan? sleepTime = null) : base(aggregateId)
        {
            SleepTime = sleepTime ?? TimeSpan.FromMilliseconds(500);
        }

        public TimeSpan SleepTime { get; }
    }
}