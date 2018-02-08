using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.BalloonDomain.Commands
{
    public class WriteTitleCommand : Command
    {
        public WriteTitleCommand(int parameter, Guid aggregateId) : base(aggregateId)
        {
            Parameter = parameter;
        }

        public int Parameter { get; }
    }
}