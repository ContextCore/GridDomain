using System;

namespace GridDomain.Node.Actors.Sagas.Exceptions
{
    internal class SagaStateException : Exception
    {
        public SagaStateException(string message):base(message)
        {
        }
    }
}