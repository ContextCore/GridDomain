namespace GridDomain.CQRS
{
    public interface ICommandFault<out T>: ICommandFault where T : ICommand
    {
        new T Command { get; }
    }

    public interface ICommandFault : IMessageProcessFault<ICommand>
    {
        ICommand Command { get; }
    }
}