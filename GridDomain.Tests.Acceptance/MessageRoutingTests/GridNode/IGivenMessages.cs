namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode
{
    public interface IGivenMessages<T>
    {
        T[] GetCommands();
    }
}