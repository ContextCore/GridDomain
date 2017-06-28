using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.FutureEvents;

namespace GridGomain.Tests.Stress.BalloonDomain
{
    public class BalloonWithProjectionFixture : NodeTestFixture
    {
        public BalloonWithProjectionFixture()
        {
            Add(new BalloonWithProjectionDomainConfiguration(AkkaConfig.Persistence.JournalConnectionString));
            Add(new BalloonWithProjectionRouteMap());
            this.ClearSheduledJobs();
        }
    }
}