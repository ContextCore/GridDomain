using System;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.BalloonDomain.Commands
{
    public class InflateCopyCommand : Command<Balloon>
    {
        public InflateCopyCommand(int parameter, string aggregateId) : base(aggregateId)
        {
            Parameter = parameter;
        }

        public int Parameter { get; }
    }
}