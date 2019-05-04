using System;

namespace GridDomain.Node.Akka.Actors.EventSourced.Exceptions
{
    internal class EventApplyException : Exception
    {
        public EventApplyException()
        {
            
        }
        public EventApplyException(Exception exception) : base("An error occured while applying event to aggregate. State can be corrupted", exception) {}
    }
}