using System;

namespace GridDomain.Node.Actors.EventSourced.Exceptions
{
    internal class EventApplyException : Exception
    {
        public EventApplyException(Exception exception) : base("An error occured while applying event to aggregate. State can be corrupted", exception) {}
    }
}