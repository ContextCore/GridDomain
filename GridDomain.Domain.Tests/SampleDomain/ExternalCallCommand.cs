using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.SampleDomain
{
    public class AsyncMethodCommand : Command
    {
        public AsyncMethodCommand(int parameter, Guid aggregateId,Guid sagaId = default(Guid)):base(Guid.NewGuid(),sagaId)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
        public int Parameter { get; }
    }
}