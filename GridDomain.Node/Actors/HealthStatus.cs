namespace GridDomain.Node.Actors
{
    public class HealthStatus
    {
        public HealthStatus(string payload = null)
        {
            Payload = payload;
        }

        public string Payload { get; }
    }
}