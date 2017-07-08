namespace GridDomain.Node.Actors.Aggregate.Messages {
    public class CommandHandlerExecuted
    {
        private CommandHandlerExecuted()
        {

        }
        public static CommandHandlerExecuted Instance { get; } = new CommandHandlerExecuted();
    }
}