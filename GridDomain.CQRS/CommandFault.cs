using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public class MessageFault<T> : IMessageFault<T>
    {
        public Guid Id { get; }
        public Exception Exception { get; }
        public Guid SagaId { get; }
        public DateTime OccuredTime { get; }
        public T Message { get; }
        object IMessageFault.Message => Message;

        public MessageFault(Guid id, T msg, Exception ex, DateTime occuredTime)
        {
            Id = id;
            Message = msg;
            Exception = ex;
            OccuredTime = occuredTime;
        }
        public MessageFault(Guid id, T msg, Exception ex):this(id,msg,ex,BusinessDateTime.UtcNow)
        {
        }
    }

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