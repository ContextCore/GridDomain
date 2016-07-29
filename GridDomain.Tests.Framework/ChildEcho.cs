namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    internal class ChildEcho
    {
        public object Message { get; set; }

        public ChildEcho(object message)
        {
            Message = message;
        }
    }
}