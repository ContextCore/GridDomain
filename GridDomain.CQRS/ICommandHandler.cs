namespace GridDomain.CQRS
{
    public interface ICommandHandler<in TCommand> : IHandler<TCommand>
    {
    }
}