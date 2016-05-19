using System;

namespace GridDomain.Tests.Acceptance
{
    internal class UnbindedMessageRecievedException : Exception
    {
        public readonly object Msg;

        public UnbindedMessageRecievedException(object message)
        {
            Msg = message;
        }
    }
}