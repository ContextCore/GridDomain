using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.FutureEvents;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonWithProjectionFixture : NodeTestFixture
    {
        public BalloonWithProjectionFixture()
        {
            Add(new BalloonWithProjectionContainerConfiguration(AkkaConfig.Persistence.JournalConnectionString));
            Add(new BalloonWithProjectionRouteMap());
            this.ClearSheduledJobs();
        }
    }
}