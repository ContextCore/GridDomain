namespace GridDomain.Node.Actors
{
    public class GracefullShutdownRequest
    {
        public static readonly GracefullShutdownRequest Instance = new GracefullShutdownRequest();

        private GracefullShutdownRequest() {}
    }
}