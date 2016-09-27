using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class CannotFindFactoryForSagaCreation : Exception
    {
        public Type Saga { get; set; }
        public new object Data { get; set; }

        public CannotFindFactoryForSagaCreation(Type saga,object data)
        {
            Saga = saga;
            Data = data;
        }
    }
}