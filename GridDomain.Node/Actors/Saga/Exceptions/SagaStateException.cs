using System;

namespace GridDomain.Node.Actors.Saga.Exceptions
{
    internal class SagaStateException : Exception
    {
        public SagaStateException(string message):base(message)
        {
        }
    }
}