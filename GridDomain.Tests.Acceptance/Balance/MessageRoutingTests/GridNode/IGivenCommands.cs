namespace GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode
{
    public interface IGivenCommands<T>
    {
        T[] GetCommands();
    }
}