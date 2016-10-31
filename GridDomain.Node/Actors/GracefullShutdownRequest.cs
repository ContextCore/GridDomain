namespace GridDomain.Node.Actors
{
    class GracefullShutdownRequest
    {
        private GracefullShutdownRequest()
        {
        }

        public static GracefullShutdownRequest Instance = new GracefullShutdownRequest();
    }
}