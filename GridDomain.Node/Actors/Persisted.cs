namespace GridDomain.Node.Actors
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