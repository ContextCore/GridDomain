using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure
{
    public class BoomNowCommand : Command
    {
        public BoomNowCommand(Guid aggregateId)
            : base(aggregateId)
        {
    
        }


    }
}