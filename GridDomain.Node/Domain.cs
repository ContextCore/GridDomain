using GridDomain.Aggregates;

namespace GridDomain.Node
{
    public class Domain:IDomain
    {
        public Domain(ICommandHandler<ICommand> commandExecutor, IAggregatesLifetime aggregatesLifetime)
        {
            CommandExecutor = commandExecutor;
            AggregatesLifetime = aggregatesLifetime;
        }

        public ICommandHandler<ICommand> CommandExecutor { get; }
        public IAggregatesLifetime AggregatesLifetime { get; }
    }
}