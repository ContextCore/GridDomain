using System;

namespace GridDomain.Node.Actors
{
    internal class EventApplyException : Exception
    {
        public EventApplyException(Exception exception) : base("An error occured while applying event to aggregate. State can be corrupted", exception) {}
    }
}