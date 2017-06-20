using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.XUnit.FutureEvents;

namespace GridDomain.Tests.Acceptance.XUnit.BalloonDomain
{
    public class BalloonWithProjectionFixture : NodeTestFixture
    {
        public BalloonWithProjectionFixture()
        {
            Add(new BalloonWithProjectionContainerConfiguration());
            Add(new BalloonWithProjectionRouteMap());
            this.ClearSheduledJobs();
        }
    }
}