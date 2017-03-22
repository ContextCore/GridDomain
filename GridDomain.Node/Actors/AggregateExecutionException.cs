using System;

namespace GridDomain.Node.Actors
{
    internal class AggregateExecutionException : Exception
    {
        public AggregateExecutionException(Exception exception) : base("Exception was raised during execution of continuation in aggregate method", exception) {}
    }
}