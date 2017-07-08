namespace GridDomain.Node.Actors.Aggregate.Messages {
    public class ProducedEventsPersisted
    {
        private ProducedEventsPersisted()
        {
            
        }
        public static ProducedEventsPersisted Instance { get; } = new ProducedEventsPersisted();
    }
}