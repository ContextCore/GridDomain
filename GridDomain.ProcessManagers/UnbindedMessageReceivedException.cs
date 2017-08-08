using System;

namespace GridDomain.ProcessManagers
{
    public class UnbindedMessageReceivedException : Exception
    {
        public readonly object Msg;

        public UnbindedMessageReceivedException(object message, Type failedType = null)
        {
            FailedType = failedType;
            Msg = message;
        }

        public Type FailedType { get; }
    }
}