using GridDomain.Aggregates;

namespace GridDomain.Node
{
    public interface IDomain
    {
        ICommandHandler<ICommand> CommandExecutor { get; }
    }

    public class Domain:IDomain
    {
        public Domain(ICommandHandler<ICommand> commandExecutor)
        {
            CommandExecutor = commandExecutor;
        }

        public ICommandHandler<ICommand> CommandExecutor { get; }
    }
}