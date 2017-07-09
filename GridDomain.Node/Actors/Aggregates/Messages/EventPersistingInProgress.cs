namespace GridDomain.Node.Actors.Aggregates.Messages {
    public class EventPersistingInProgress
    {
        public EventPersistingInProgress()
        {

        }
        public static EventPersistingInProgress Instance { get; } = new EventPersistingInProgress();
    }
}