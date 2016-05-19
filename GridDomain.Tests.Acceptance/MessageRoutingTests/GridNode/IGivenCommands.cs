namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode
{
    public interface IGivenCommands<T>
    {
        T[] GetCommands();
    }
}