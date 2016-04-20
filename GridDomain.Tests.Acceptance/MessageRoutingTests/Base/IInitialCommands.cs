namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    interface IInitialCommands<T>
    {
        T[] GetCommands();
    }
}