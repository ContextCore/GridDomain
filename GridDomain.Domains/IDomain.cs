using GridDomain.Aggregates;

namespace GridDomain.Domains
{
    public interface IDomain
    {
        ICommandHandler<ICommand> CommandExecutor { get; }
        IAggregatesController AggregatesController { get; }
    }
}