using System;

namespace GridDomain.Node.Actors
{
    internal class CannotGetSagaFromMessage : Exception
    {
        public object Msg { get;}

        public CannotGetSagaFromMessage(object msg)
        {
            Msg = msg;
        }
    }
}