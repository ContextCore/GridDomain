namespace GridDomain.Node.Actors.Aggregate.Messages
{
    public class NotifyOnCommandComplete
    {
        private NotifyOnCommandComplete() {}
        public static NotifyOnCommandComplete Instance = new NotifyOnCommandComplete();
    }
}