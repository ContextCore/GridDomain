using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.BalloonDomain.Commands
{
    public class PlanTitleWriteAndBlowCommand : Command
    {
        public PlanTitleWriteAndBlowCommand(int parameter,
                                            string aggregateId,
                                             TimeSpan? sleepTime = null) : base(aggregateId)
        {
            Parameter = parameter;
            SleepTime = sleepTime ?? TimeSpan.FromSeconds(1);
        }
        
        public PlanTitleWriteAndBlowCommand(int parameter,
                                            Guid aggregateId,
                                            TimeSpan? sleepTime = null) : this(parameter,aggregateId.ToString(),sleepTime)
        {
        }

        public TimeSpan SleepTime { get; }
        public int Parameter { get; }
    }
}