using Akka.Actor;
using GridDomain.Aggregates;

namespace GridDomain.Node
{
    public interface IDomain
    {
        ICommandHandler<ICommand> CommandExecutor { get; }
        IAggregatesLifetime AggregatesLifetime { get; }
    }
}