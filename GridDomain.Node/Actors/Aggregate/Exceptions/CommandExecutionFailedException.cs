using System;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.Aggregate.Exceptions
{
    public class CommandExecutionFailedException : Exception
    {
        public CommandExecutionFailedException(ICommand command, Exception innerException)
            : base("Command execution failed", innerException)
        {
            Command = command;
        }

        public ICommand Command { get; }
    }
}