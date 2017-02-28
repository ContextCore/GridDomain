using System;

namespace GridDomain.Node.Actors
{
    internal class CannotGetSagaFromMessage : Exception
    {
        public CannotGetSagaFromMessage(object msg)
        {
            Msg = msg;
        }

        public object Msg { get; }
    }
}