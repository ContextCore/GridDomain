using System;
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

    public class CommandExecutionFailedException : Exception
    {
        public ICommand Command { get; }

        public CommandExecutionFailedException(ICommand command,Exception innerException):base("Command execution failed", innerException)
        {
            Command = command;
        }
    }
}