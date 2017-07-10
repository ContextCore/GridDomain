namespace GridDomain.Node.Actors.Aggregates.Messages {
    class EventsPersisted
    {

        private EventsPersisted()
        {
        }

        public static EventsPersisted Instance { get; } = new EventsPersisted();
    }
}