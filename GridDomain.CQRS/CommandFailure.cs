namespace GridDomain.CQRS
{
    public class CommandFailure<TCommand, TReason> : ICommandFault<TCommand> where TCommand : ICommand
    {
        public CommandFailure(TCommand command, TReason ex)
        {
            Error = ex;
            Command = command;
        }

        public TReason Error { get; }
        public TCommand Command { get; }
        public object Fault => Error;
    }
}