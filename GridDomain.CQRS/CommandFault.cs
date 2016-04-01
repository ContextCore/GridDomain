namespace GridDomain.CQRS
{
    public class CommandFault<T>
    {
        public T Command { get; }
        public object Fault { get; }

        public CommandFault(T command, object fault)
        {
            Command = command;
            Fault = fault;
        }

    }
}