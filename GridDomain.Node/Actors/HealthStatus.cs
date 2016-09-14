namespace GridDomain.Node.Actors
{
    class HealthStatus
    {
        public string Payload { get; }

        public HealthStatus(string payload = null)
        {
            this.Payload = payload;
        }
    }
}