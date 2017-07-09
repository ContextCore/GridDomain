namespace GridDomain.Node.Actors.Aggregates.Messages {
    public class CommandExecuted
    {
        private CommandExecuted()
        {

        }
        public static CommandExecuted Instance { get; } = new CommandExecuted();
    }
}