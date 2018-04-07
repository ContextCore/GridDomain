using System;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure {
    public class PlanBoomCommand : Command<TestFutureEventsAggregate>
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