using System;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.BalloonDomain {
    public class DoubleIncreaseTitleCommand : Command<Balloon>
    {
        public DoubleIncreaseTitleCommand(int value, string aggregateId) : base(aggregateId)
        {
            Value = value;
        }

        public int Value { get; }
    }
}