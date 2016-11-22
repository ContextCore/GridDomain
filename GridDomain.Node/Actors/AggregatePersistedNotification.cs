using System;

namespace GridDomain.Node.Actors
{
    public class Persisted
    {
        public object Event { get;}

        public Persisted(object @event)
        {
            Event = @event;
        }
    }
}