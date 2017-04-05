using GridDomain.Tests.XUnit.BalloonDomain;

namespace GridDomain.Tests.XUnit
{
    public class BalloonFixture : NodeTestFixture
    {
        public BalloonFixture()
        {
            Add(new BalloonContainerConfiguration());
            Add(new BalloonRouteMap());
        }
    }
}