namespace GridDomain.Node.Actors.EventSourced.Messages
{
    public class GracefullShutdownRequest
    {
        public static readonly GracefullShutdownRequest Instance = new GracefullShutdownRequest();

        private GracefullShutdownRequest() {}
    }
}