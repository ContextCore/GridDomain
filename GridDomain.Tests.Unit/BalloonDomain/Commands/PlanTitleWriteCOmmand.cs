using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.BalloonDomain.Commands
{
    public class PlanTitleWriteCommand : Command
    {
        public PlanTitleWriteCommand(int parameter, Guid aggregateId) : base(aggregateId)
        {
            Parameter = parameter;
        }

        public int Parameter { get; }
    }
}