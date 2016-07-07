using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    public class ChangeAggregateCommand : Command
    {
        public int Parameter { get; }

        public Guid AggregateId { get; }

        public ChangeAggregateCommand(Guid aggregateId, int parameter)
        {
            AggregateId = aggregateId;
            Parameter = parameter;
        }
    }
}