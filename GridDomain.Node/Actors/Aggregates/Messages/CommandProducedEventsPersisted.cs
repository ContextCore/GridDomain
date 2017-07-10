namespace GridDomain.Node.Actors.Aggregates.Messages {
    public class CommandProducedEventsPersisted
    {
        private CommandProducedEventsPersisted()
        {
            
        }
        public static CommandProducedEventsPersisted Instance { get; } = new CommandProducedEventsPersisted();
    }
}