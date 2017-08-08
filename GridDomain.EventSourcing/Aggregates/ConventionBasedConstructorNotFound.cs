using System;

namespace GridDomain.EventSourcing.Aggregates
{
    public class ConventionBasedConstructorNotFound : Exception
    {
        public ConventionBasedConstructorNotFound() : base("Cannot find private constructor with single Id parameter") {}
    }
}