using System;

namespace GridDomain.CQRS
{
    public static class CommandFaultFactory
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