using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class FactoryAlreadyRegisteredException : Exception
    {
        public FactoryAlreadyRegisteredException(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }
    }
}