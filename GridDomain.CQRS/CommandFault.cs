using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public class CommandFault<TCommand> : ICommandFault<TCommand>
                                         where TCommand : ICommand
    {
        public CommandFault(TCommand command, Exception ex, DateTime occuredTime)
        {
            Exception = ex;
            OccuredTime = occuredTime;
            Command = command;
            SagaId = command.SagaId;
        }

        public CommandFault(TCommand command, Exception ex) : this(command, ex, BusinessDateTime.UtcNow) 
        {
            
        }

        public Guid Id => Command.Id;

        public Exception Exception { get; }
        public Guid SagaId { get; }
        public DateTime OccuredTime { get; }

        public TCommand Command { get; }
        public ICommand Message => Command;
        ICommand ICommandFault.Command => Command;
        object IMessageProcessFault.Message => Command;
    }
}