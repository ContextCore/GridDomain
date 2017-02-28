using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class CannotFindFactoryForSagaCreation : Exception
    {
        public CannotFindFactoryForSagaCreation(Type saga, object data)
        {
            Saga = saga;
            Data = data;
        }

        public Type Saga { get; set; }
        public new object Data { get; set; }
    }
}