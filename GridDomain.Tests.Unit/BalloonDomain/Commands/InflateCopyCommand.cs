using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.BalloonDomain.Commands
{
    public class InflateCopyCommand : Command
    {
        public InflateCopyCommand(int parameter, string aggregateId) : base(aggregateId)
        {
            Parameter = parameter;
        }

        public int Parameter { get; }
    }
}