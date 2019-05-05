using GridDomain.Aggregates;

namespace GridDomain.Domains
{
    public interface IDomain
    {
        ICommandHandler<ICommand> CommandExecutor { get; }
        T CommandHandler<T>() where T : ICommandHandler<ICommand>;
        IAggregatesController AggregatesController { get; }
    }
}