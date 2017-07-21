using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class StartMessagesMissedException : Exception
    {
        public StartMessagesMissedException() : base("Saga descriptor should contains at least one start message") {}
    }
}