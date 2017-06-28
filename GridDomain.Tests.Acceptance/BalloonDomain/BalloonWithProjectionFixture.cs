using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonWithProjectionFixture : NodeTestFixture
    {
        public BalloonWithProjectionFixture()
        {
            Add(new BalloonWithProjectionDomainConfiguration(AkkaConfig.Persistence.JournalConnectionString));
            this.ClearSheduledJobs();
        }
    }
}