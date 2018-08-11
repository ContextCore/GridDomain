using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.BalloonDomain.Commands
{
    public class BlowBalloonCommand : Command<Balloon>,IFor<BalloonCommandHandler>
    {
        public BlowBalloonCommand(string aggregateId) : base(aggregateId) {}
    }
}