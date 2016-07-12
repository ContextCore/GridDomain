using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.SampleDomain
{
    public class AlwaysFaultAsyncCommand : Command
    {
        public AlwaysFaultAsyncCommand(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
    }
}