using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.BalloonDomain.Commands
{
    public class InflateNewBallonCommand : Command<Balloon>
    {
        public InflateNewBallonCommand(int title, string aggregateId) : base(aggregateId)
        {
            Title = title;
        }

        public int Title { get; }
    }
}