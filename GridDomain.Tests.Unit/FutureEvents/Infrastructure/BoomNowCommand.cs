using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure
{
    public class BoomNowCommand : Command<TestFutureEventsAggregate>
    {
        public BoomNowCommand(string aggregateId)
            : base(aggregateId)
        {
    
        }


    }
}