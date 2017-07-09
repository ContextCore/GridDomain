namespace GridDomain.Node.Actors.Aggregates.Messages {
    public class ProducedEventsPersisted
    {
        private ProducedEventsPersisted()
        {
            
        }
        public static ProducedEventsPersisted Instance { get; } = new ProducedEventsPersisted();
    }
}