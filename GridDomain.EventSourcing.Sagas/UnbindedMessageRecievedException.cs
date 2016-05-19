using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class UnbindedMessageRecievedException : Exception
    {
        public readonly object Msg;

        public UnbindedMessageRecievedException(object message)
        {
            Msg = message;
        }
    }
}