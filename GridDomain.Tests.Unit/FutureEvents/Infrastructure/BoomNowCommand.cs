using System;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

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