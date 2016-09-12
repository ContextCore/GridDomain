using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public class CommandFault<TCommand> : MessageFault<TCommand>, 
                                          IMessageFault<TCommand>
                                          where TCommand : ICommand
    {
        public CommandFault(TCommand command, Exception ex, DateTime occuredTime):base(command.Id,command,ex,occuredTime)
        {
        }

        public CommandFault(TCommand command, Exception ex) : this(command, ex, BusinessDateTime.UtcNow) 
        {
            
        }

        public TCommand Command { get; }
        public ICommand Message { get; }
    }
}