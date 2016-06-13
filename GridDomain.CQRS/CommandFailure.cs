using System;
using System.Reflection;

namespace GridDomain.CQRS
{
    public static class CommandFault
    {
        public static object CreateGenericFor(ICommand command, Exception ex)
        {
            var type = command.GetType();
            var faultType = typeof(CommandFault<>).MakeGenericType(type);
            var fault = faultType.GetConstructor(new[] { type, typeof(Exception)})
                                 .Invoke(new object[] { command, ex });
            return fault;
        }
    }
    public class CommandFault<TCommand> : ICommandFault<TCommand>
                                    where TCommand : ICommand
    {
        public CommandFault(TCommand command, Exception ex)
        {
            Fault = ex;
            Command = command;
            SagaId = command.SagaId;
        }

        ICommand ICommandFault.Command => Command;

        public Exception Fault { get; }
        public Guid SagaId { get; } 

        public TCommand Command { get; }

    }
}