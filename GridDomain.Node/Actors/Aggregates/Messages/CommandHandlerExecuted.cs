namespace GridDomain.Node.Actors.Aggregates.Messages {
    public class CommandHandlerExecuted
    {
        private CommandHandlerExecuted()
        {

        }
        public static CommandHandlerExecuted Instance { get; } = new CommandHandlerExecuted();
    }
}