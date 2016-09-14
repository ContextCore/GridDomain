using GridDomain.CQRS;

namespace GridDomain.Node.Actors
{
    public class CommandExecutionFinished
    {
        public object ResultMessage { get; }
        public ICommand Command { get; }

        public CommandExecutionFinished(ICommand command, object resultMessage)
        {
            ResultMessage = resultMessage;
            Command = command;
        }
    }
}