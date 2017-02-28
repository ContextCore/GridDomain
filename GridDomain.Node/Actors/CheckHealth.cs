namespace GridDomain.Node.Actors
{
    internal class CheckHealth
    {
        public static CheckHealth Instance = new CheckHealth();

        public CheckHealth(string payload = null)
        {
            Payload = payload;
        }

        public string Payload { get; }
    }
}