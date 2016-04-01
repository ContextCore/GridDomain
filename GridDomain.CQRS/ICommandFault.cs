namespace GridDomain.CQRS
{
    public interface ICommandFault<T> where T : ICommand
    {
        T Command { get; }
        object Fault { get; }
    }
}