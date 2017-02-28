using System;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class StartMessagesMissedException : Exception
    {
        public StartMessagesMissedException() : base("Saga descriptor should contains at least one start message") {}
    }
}