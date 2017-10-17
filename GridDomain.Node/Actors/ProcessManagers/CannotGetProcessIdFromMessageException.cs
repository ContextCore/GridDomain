using System;

namespace GridDomain.Node.Actors.ProcessManagers {
    internal class CannotGetProcessIdFromMessageException : Exception
    {
        public object Msg { get; }

        public CannotGetProcessIdFromMessageException(object msg)
        {
            Msg = msg;
        }
    }
}