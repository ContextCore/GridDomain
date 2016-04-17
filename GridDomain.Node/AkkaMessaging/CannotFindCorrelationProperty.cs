using System;

namespace GridDomain.Node.AkkaMessaging
{
    class CannotFindCorrelationProperty : Exception
    {
        public Type Type { get; set; }
        public string Property { get; set; }

        public CannotFindCorrelationProperty(Type type, string property):
            base($"Cannot find property {property} in type {type}")
        {
            Type = type;
            Property = property;
        }
    }
}