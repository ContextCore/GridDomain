using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.FutureEvents;

namespace GridDomain.Tests.Unit
{
    public class BalloonFixture : NodeTestFixture
    {
        public BalloonFixture()
        {
            Add(new BalloonContainerConfiguration());
            Add(new BalloonRouteMap());
            this.ClearSheduledJobs();
        }
    }
}