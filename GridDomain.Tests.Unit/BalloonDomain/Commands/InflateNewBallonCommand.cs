using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.BalloonDomain.Commands
{
    public class InflateNewBallonCommand : Command<Balloon>, IFor<BalloonCommandHandler>
    {
        public InflateNewBallonCommand(int title, string aggregateId) : base(aggregateId)
        {
            Title = title;
        }

        public int Title { get; }
    }
}