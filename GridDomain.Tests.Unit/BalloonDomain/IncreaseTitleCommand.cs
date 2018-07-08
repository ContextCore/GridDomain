using System;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.BalloonDomain
{
    public class IncreaseTitleCommand : Command<Balloon>
    {
        public IncreaseTitleCommand(int value, string aggregateId) : base(aggregateId)
        {
            Value = value;
        }

        public int Value { get; }
    }
}