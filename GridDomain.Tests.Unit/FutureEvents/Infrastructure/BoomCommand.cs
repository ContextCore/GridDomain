using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure {
    public class PlanBoomCommand : Command
    {
        public PlanBoomCommand(Guid aggregateId, DateTime boomTime):base(aggregateId)
        {
            BoomTime = boomTime;
        }

        public DateTime BoomTime { get;}
    }
}