namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    class Pong
    {
        public Pong(object payload)
        {
            Payload = payload;
        }

        public object Payload { get; }
    }
}