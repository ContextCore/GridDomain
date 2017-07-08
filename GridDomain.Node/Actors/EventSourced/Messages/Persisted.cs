namespace GridDomain.Node.Actors.EventSourced.Messages
{
    public class Persisted
    {
        public Persisted(object @event)
        {
            Event = @event;
        }

        public object Event { get; }
    }
}