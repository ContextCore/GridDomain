using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.BalloonDomain.Commands
{
    public class InflateNewBallonCommand : Command
    {
        public InflateNewBallonCommand(int title, Guid aggregateId) : base(aggregateId)
        {
            Title = title;
        }

        public int Title { get; }
    }
}