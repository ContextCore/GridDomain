namespace GridDomain.CQRS
{
    public class CommandFailure<TCommand,TReason>:ICommandFault<TCommand> where TCommand : ICommand
    {
        public TCommand Command { get; private set; }
        public object Fault => Error;
        public TReason Error { get; private set; }

        public CommandFailure(TCommand command, TReason ex)
        {
            Error = ex;
            Command = command;
        }

    }
}