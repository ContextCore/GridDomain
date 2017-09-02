using System;

namespace GridDomain.ProcessManagers.Creation
{
    public class FactoryNotSupportStartMessageException : Exception
    {
        public FactoryNotSupportStartMessageException()
        {
            
        }
        public FactoryNotSupportStartMessageException(Type factoryType, Type startMessageType)
        {
            FactoryType = factoryType;
            StartMessageType = startMessageType;
        }

        public Type FactoryType { get; }
        public Type StartMessageType { get; }
    }
}