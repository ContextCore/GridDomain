using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.BalloonDomain.Commands
{
    public class PlanTitleChangeCommand : Command
    {
        public PlanTitleChangeCommand(int parameter, string aggregateId, string processId = null, TimeSpan? sleepTime = null)
            : base(Guid.NewGuid().ToString(), aggregateId, processId)
        {
            Parameter = parameter;
            SleepTime = sleepTime ?? TimeSpan.FromSeconds(1);
        }
        
        public PlanTitleChangeCommand(int parameter, Guid aggregateId, Guid? processId = null, TimeSpan? sleepTime = null)
            : base(Guid.NewGuid().ToString(), aggregateId.ToString(), processId?.ToString())
        {
            Parameter = parameter;
            SleepTime = sleepTime ?? TimeSpan.FromSeconds(1);
        }


        public TimeSpan SleepTime { get; }
        public int Parameter { get; }
    }
}