using System;
using System.Reflection;

namespace GridDomain.CQRS
{
    public class CommandFault<TCommand> : ICommandFault<TCommand>
                                    where TCommand : ICommand
    {
        public CommandFault(TCommand command, Exception ex)
        {
            Exception = ex;
            Command = command;
            SagaId = command.SagaId;
        }

        ICommand ICommandFault.Command => Command;

        public Exception Exception { get; }
        public Guid SagaId { get; } 

        public TCommand Command { get; }

    }
}