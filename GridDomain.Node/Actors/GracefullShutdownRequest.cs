namespace GridDomain.Node.Actors
{
    public class GracefullShutdownRequest
    {
        private GracefullShutdownRequest()
        {
        }

        public static readonly GracefullShutdownRequest Instance = new GracefullShutdownRequest();
    }

    public class CancelShutdownRequest
    {
        private CancelShutdownRequest()
        {
        }

        public static readonly CancelShutdownRequest Instance = new CancelShutdownRequest();
    }


}