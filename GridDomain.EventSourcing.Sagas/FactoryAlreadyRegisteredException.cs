using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class FactoryAlreadyRegisteredException : Exception
    {
        public Type Type { get; set; }

        public FactoryAlreadyRegisteredException(Type type)
        {
            Type = type;
        }
    }
}