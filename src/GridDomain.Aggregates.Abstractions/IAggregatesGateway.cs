using GridDomain.Abstractions;

namespace GridDomain.Aggregates.Abstractions
{
    public interface IAggregatesGateway:IDomainPart
    {
        ICommandHandler<ICommand> CommandExecutor { get; }
        T CommandHandler<T>() where T : ICommandHandler<ICommand>;
    }
}