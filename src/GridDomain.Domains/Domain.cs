using GridDomain.Aggregates;

namespace GridDomain.Domains
{
    public class Domain:IDomain
    {
        public Domain(ICommandHandler<ICommand> commandExecutor, IAggregatesController aggregatesController)
        {
            CommandExecutor = commandExecutor;
            AggregatesController = aggregatesController;
        }

        public ICommandHandler<ICommand> CommandExecutor { get; }
        public IAggregatesController AggregatesController { get; }
    }
}