using System;

namespace GridDomain.Node.Actors.ProcessManagers {
    internal class MessageReceivedOnTerminationException : Exception
    {
        public MessageReceivedOnTerminationException() : base("Received a new message to process after State asked to terminate") { }
    }
}