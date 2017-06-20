using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.FutureEvents;

namespace GridGomain.Tests.Stress.BalloonDomain
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