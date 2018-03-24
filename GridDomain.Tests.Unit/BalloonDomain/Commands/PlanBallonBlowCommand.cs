using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.BalloonDomain.Commands
{
    public class PlanBallonBlowCommand : Command<Balloon>
    {
        public PlanBallonBlowCommand(string aggregateId, TimeSpan? sleepTime = null) : base(aggregateId)
        {
            SleepTime = sleepTime ?? TimeSpan.FromMilliseconds(500);
        }

        public TimeSpan SleepTime { get; }
    }
}