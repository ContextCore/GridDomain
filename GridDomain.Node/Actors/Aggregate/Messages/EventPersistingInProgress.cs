namespace GridDomain.Node.Actors.Aggregate.Messages {
    public class EventPersistingInProgress
    {
        public EventPersistingInProgress()
        {

        }
        public static EventPersistingInProgress Instance { get; } = new EventPersistingInProgress();
    }
}