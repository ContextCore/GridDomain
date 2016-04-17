using System;

namespace GridDomain.Node.AkkaMessaging
{
    internal class IncorrectTypeOfCorrelationProperty : Exception
    {
        public Type Type { get; set; }
        public string Property { get; set; }

        public IncorrectTypeOfCorrelationProperty(Type type, string property):
            base($"Correlation property {property} of type {type} should be {typeof(Guid)} type to act as correlation property")
        {
            Type = type;
            Property = property;
        }
    }
}