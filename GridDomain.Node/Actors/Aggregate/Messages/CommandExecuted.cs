namespace GridDomain.Node.Actors.Aggregate.Messages {
    public class CommandExecuted
    {
        private CommandExecuted()
        {

        }
        public static CommandExecuted Instance { get; } = new CommandExecuted();
    }
}