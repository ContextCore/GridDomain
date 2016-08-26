namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    class Ping
    {
        public object Payload { get; }

        public Ping(object payload)
        {
            Payload = payload;
        }
    }
}