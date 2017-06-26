using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.BalloonDomain.Commands
{
    public class BlowBalloonCommand : Command
    {
        public BlowBalloonCommand(Guid aggregateId) : base(aggregateId) {}
    }
}