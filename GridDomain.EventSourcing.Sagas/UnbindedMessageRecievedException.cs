using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class UnbindedMessageReceivedException : Exception
    {
        public Type FailedType { get; }
        public readonly object Msg;

        public UnbindedMessageReceivedException(object message, Type failedType = null)
        {
            FailedType = failedType;
            Msg = message;
        }
    }
}