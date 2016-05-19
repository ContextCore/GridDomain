using System;

namespace GridDomain.Tests.Acceptance
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