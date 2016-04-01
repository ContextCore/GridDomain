namespace GridDomain.CQRS
{
    public class CommandFault<T>
    {
        public CommandFault(T command, object fault)
        {
            Command = command;
            Fault = fault;
        }

        public T Command { get; }
        public object Fault { get; }
    }
}