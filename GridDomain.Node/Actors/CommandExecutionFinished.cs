using GridDomain.CQRS;

namespace GridDomain.Node.Actors
{
    public class CommandExecutionFinished
    {
        public object ResultMessage { get; }
        public ICommand CommandId { get; }

        public CommandExecutionFinished(ICommand commandId, object resultMessage)
        {
            ResultMessage = resultMessage;
            CommandId = commandId;
        }
    }
}