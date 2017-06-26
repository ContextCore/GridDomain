using GridDomain.Tests.XUnit.BalloonDomain;
using GridDomain.Tests.XUnit.FutureEvents;

namespace GridDomain.Tests.XUnit
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