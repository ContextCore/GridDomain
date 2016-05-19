using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup
{
    public abstract class SingleActorSystemTest : ActorSystemTest<TestMessage, SingleActorSystemInfrastructure>
    {
        protected override SingleActorSystemInfrastructure CreateInfrastructure()
        {
            return new SingleActorSystemInfrastructure(new AkkaConfiguration("test", 9000, "127.0.0.1"));

        }
        protected override IGivenCommands<TestMessage> GivenCommands()
        {
            return new GivenTestMessages();
        }
    }
}