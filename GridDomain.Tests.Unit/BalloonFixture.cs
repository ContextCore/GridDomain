using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.FutureEvents;

namespace GridDomain.Tests.Unit
{
    public class BalloonFixture : NodeTestFixture
    {
        public BalloonFixture(string connString)
        {
            Add(new BalloonDomainConfiguration());
            Add(new BalloonRouteMap());
            this.ClearSheduledJobs();
        }
    }
}