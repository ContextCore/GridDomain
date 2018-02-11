using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.BalloonDomain {
    public class DoubleIncreaseTitleCommand : Command
    {
        public DoubleIncreaseTitleCommand(int value, string aggregateId) : base(aggregateId)
        {
            Value = value;
        }

        public int Value { get; }
    }
}