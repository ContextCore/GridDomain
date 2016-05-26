namespace GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode.SingleSystem.Setup
{
    public abstract class SingleActorSystemTest : ActorSystemTest<TestMessage, SingleActorSystemInfrastructure>
    {
        protected override SingleActorSystemInfrastructure CreateInfrastructure()
        {
            return new SingleActorSystemInfrastructure(new AutoTestAkkaConfiguration());

        }
        protected override IGivenCommands<TestMessage> GivenCommands()
        {
            return new GivenTestMessages();
        }
    }
}