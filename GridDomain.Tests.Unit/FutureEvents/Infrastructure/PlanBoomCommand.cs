using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure {
    public class PlanBoomCommand : Command
    {
        public PlanBoomCommand(string aggregateId, DateTime boomTime):base(aggregateId)
        {
            BoomTime = boomTime;
        }
        public PlanBoomCommand(Guid aggregateId, DateTime boomTime):this(aggregateId.ToString(),boomTime)
        {
        }

        public DateTime BoomTime { get;}
    }
}