using System;

namespace GridDomain.Node.AkkaMessaging
{
    internal class InvalidCorrelationPropertyValue : Exception
    {
        public InvalidCorrelationPropertyValue(object value) : base(value?.ToString())
        {
        }
    }
}