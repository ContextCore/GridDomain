using System;

namespace GridDomain.Node.AkkaMessaging
{
    internal class CannotFindCorrelationProperty : Exception
    {
        public CannotFindCorrelationProperty(Type type, string property)
            : base($"Cannot find property {property} in type {type}")
        {
            Type = type;
            Property = property;
        }

        public Type Type { get; set; }
        public string Property { get; set; }
    }
}