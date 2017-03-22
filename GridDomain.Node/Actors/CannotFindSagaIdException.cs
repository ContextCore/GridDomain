using System;

namespace GridDomain.Node.Actors
{
    internal class CannotFindSagaIdException : Exception
    {
        public object Msg { get; }

        public CannotFindSagaIdException(object msg)
        {
            Msg = msg;
        }
    }
}