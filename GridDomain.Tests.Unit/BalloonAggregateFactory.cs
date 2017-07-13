using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.BalloonDomain;

namespace GridDomain.Tests.Unit {
    internal class BalloonAggregateFactory : AggregatesSnapshotsFactory
    {
        public BalloonAggregateFactory()
        {
            Register(Balloon.FromSnapshot);
        }
    }
}