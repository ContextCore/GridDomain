using System;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain {
    public class CannotCreateStateFromMessageException : Exception
    {
        public object Msg { get; }

        public CannotCreateStateFromMessageException(object msg)
        {
            Msg = msg;
        }
    }
}