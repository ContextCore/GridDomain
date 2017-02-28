using System;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class NullMessageTransitException : Exception
    {
        public readonly object SagaData;

        public NullMessageTransitException(object sagaData) : base("Saga was transitioned by null message")
        {
            SagaData = sagaData;
        }
    }
}