using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.BalloonDomain.Commands
{
    public class BlowBalloonCommand : Command<Balloon>,IFor<BalloonCommandHandler>
    {
        public BlowBalloonCommand(string aggregateId) : base(aggregateId) {}
    }
}