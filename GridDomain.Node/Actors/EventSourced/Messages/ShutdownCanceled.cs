namespace GridDomain.Node.Actors.EventSourced.Messages
{
    public class ShutdownCanceled
    {
        private ShutdownCanceled() {}

        public static ShutdownCanceled Instance { get; } = new ShutdownCanceled();
    }
}