namespace GridDomain.Node.Actors
{
    public class GracefullShutdownRequest
    {
        private GracefullShutdownRequest()
        {
        }

        public static GracefullShutdownRequest Instance = new GracefullShutdownRequest();
    }
}