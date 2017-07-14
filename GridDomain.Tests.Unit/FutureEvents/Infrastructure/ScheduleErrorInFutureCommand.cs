using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure
{
    public class BoomNowCommand : Command
    {
        public BoomNowCommand(Guid aggregateId)
            : base(aggregateId)
        {
    
        }


    }
}