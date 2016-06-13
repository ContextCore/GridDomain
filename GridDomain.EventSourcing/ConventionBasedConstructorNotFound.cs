using System;

namespace GridDomain.EventSourcing
{
    public class ConventionBasedConstructorNotFound : Exception
    {
        public ConventionBasedConstructorNotFound()
            : base("Cannot find private constructor with single Id parameter")
        {
        }
    }
}