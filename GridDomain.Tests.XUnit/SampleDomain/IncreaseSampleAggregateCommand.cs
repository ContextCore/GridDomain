using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.SampleDomain
{
    public class IncreaseSampleAggregateCommand : Command
    {
        public IncreaseSampleAggregateCommand(int value, Guid aggregateId) : base(aggregateId)
        {
            Value = value;
        }

        public int Value { get; }
    }
}