using System;

namespace GridDomain.Aggregates
{
    public class UnknownCommandException : Exception
    {
    }
    
    public class AggregateVersionMismatchException : Exception
    {
    }
}