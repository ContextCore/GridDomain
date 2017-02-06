namespace GridDomain.Node.Actors
{
    public class HealthStatus
    {
        public string Payload { get; }

        public HealthStatus(string payload = null)
        {
            this.Payload = payload;
        }
    }
}