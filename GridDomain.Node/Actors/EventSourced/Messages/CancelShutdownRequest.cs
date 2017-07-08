namespace GridDomain.Node.Actors.EventSourced.Messages
{
    public class CancelShutdownRequest
    {
        public static readonly CancelShutdownRequest Instance = new CancelShutdownRequest();

        private CancelShutdownRequest() {}
    }
}