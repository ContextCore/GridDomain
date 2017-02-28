namespace GridDomain.Node.Actors
{
    public class GracefullShutdownRequest
    {
        private GracefullShutdownRequest()
        {
        }

        public static readonly GracefullShutdownRequest Instance = new GracefullShutdownRequest();
    }
}