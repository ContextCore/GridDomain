using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class FactoryNotSupportStartMessageException : Exception
    {
        public FactoryNotSupportStartMessageException(Type factoryType, Type startMessageType)
        {
            FactoryType = factoryType;
            StartMessageType = startMessageType;
        }

        public Type FactoryType { get; }
        public Type StartMessageType { get; }
    }
}