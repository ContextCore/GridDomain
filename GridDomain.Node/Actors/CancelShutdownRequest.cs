namespace GridDomain.Node.Actors
{
    public class CancelShutdownRequest
    {
        private CancelShutdownRequest()
        {
        }

        public static readonly CancelShutdownRequest Instance = new CancelShutdownRequest();
    }
}