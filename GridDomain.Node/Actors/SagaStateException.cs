using System;

namespace GridDomain.Node.Actors
{
    internal class SagaStateException : Exception
    {
        public SagaStateException(string message):base(message)
        {
        }
    }
}