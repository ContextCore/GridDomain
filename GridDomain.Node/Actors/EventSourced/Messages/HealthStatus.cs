namespace GridDomain.Node.Actors.EventSourced.Messages
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