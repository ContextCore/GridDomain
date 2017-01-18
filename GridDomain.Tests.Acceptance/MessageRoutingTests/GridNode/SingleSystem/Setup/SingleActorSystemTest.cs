using GridDomain.Tests.Framework.Configuration;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup
{
    public abstract class SingleActorSystemTest : ActorSystemTest<TestEvent, SingleActorSystemInfrastructure>
    {
        protected override SingleActorSystemInfrastructure CreateInfrastructure()
        {
            return new SingleActorSystemInfrastructure(new AutoTestAkkaConfiguration());
        }

        protected override IGivenMessages<TestEvent> GivenCommands()
        {
            return new GivenTestMessages();
        }
    }
}